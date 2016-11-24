using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.ApplicationInsights;

[Serializable]
public class risultati
{

    public string mossa { get; set; }

    public string esito { get; set; }
}
// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
    int roundNumber = 0;
    string channel = "";
    string name = "";
    int roundResult = 0;
    int roundResultMachine = 0;
    System.Collections.Generic.List<risultati> _risultati = new System.Collections.Generic.List<risultati>();
    public BasicLuisDialog(string myChannel = "", string myUsername = "") : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {


        if (!String.IsNullOrEmpty(myChannel))
            this.channel = myChannel;
        if (!String.IsNullOrEmpty(myUsername))
            this.name = myUsername;
    }

    [LuisIntent("")]
    public async Task None(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Misunderstand");
        telemetry.Flush();
        await context.PostAsync($"Non ho capito cosa hai detto. Hai detto : {result.Query}"); //
        context.Wait(MessageReceived);
    }


    [LuisIntent("Welcome")]
    public async Task Welcome(IDialogContext context, LuisResult result)
    {
        try
        {

            TelemetryClient telemetry = new TelemetryClient();
            telemetry.TrackEvent("Welcome Game");
            telemetry.Flush();

            try
            {
                System.Collections.Generic.List<risultati> temp = context.UserData.Get<System.Collections.Generic.List<risultati>>("risultati");

                if (temp != null)

                    _risultati = temp;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Errore," + ex.Message);


            }

            var msg = context.MakeMessage();
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/paper.png", "paper.png"));
            await context.PostAsync(msg);

            if (!String.IsNullOrEmpty(name))
                await context.PostAsync($"Buongiorno," + this.name + ",  hai a disposizione due comandi regole e partita"); //
            else
                await context.PostAsync($"Buongiorno, hai a disposizione due comandi regole e partita"); //

            context.Wait(MessageReceived);

            if (_risultati != null && _risultati.Count > 0)
            {
                string listamosse = "";

                int count = 0;
                foreach (var item in _risultati)
                {
                    count++;
                    listamosse += count.ToString() + " : " + item.mossa.ToString() + " " + item.esito.ToString() + " - ";
                }

                await context.PostAsync($"Le tue ultime mosse : " + listamosse);
            }
        }
        catch (Exception)
        {


        }

    }
    [LuisIntent("Regole")]
    public async Task Regole(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Regole");
        telemetry.Flush();
        await context.PostAsync($"Le regole sono semplici, la carta vince sul sasso, la forbice vince su carta e il sasso vince sulle forbici, per iniziare scrivi partita"); //
        context.Wait(MessageReceived);
    }
    [LuisIntent("Partita")]
    public async Task Partita(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Partita");
        telemetry.Flush();
        await context.PostAsync($"Iniziamo la partita che è composta da 3 round, vince chi se ne aggiudica 2. Inizia scrivendo avvia partita"); //
        context.Wait(MessageReceived);
    }
    [LuisIntent("AvviaPartita")]
    public async Task AvviaPartita(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Avvia Partita");
        telemetry.Flush();
        roundNumber = 1;
        await context.PostAsync($"Primo round, fai la tua mossa");
        context.Wait(MessageReceived);
    }
    [LuisIntent("Mossa")]
    public async Task Mossa(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Mossa");
        telemetry.Flush();

        EntityRecommendation tipomossa;
        if (!result.TryFindEntity("tipomossa", out tipomossa))
        {
            tipomossa = new EntityRecommendation(type: "tipomossa") { Entity = "" };
        }
        if (tipomossa.Entity == "")
        {
            await context.PostAsync($"Non ho capito la tua mossa,digita ad esempio lancio sasso"); //
            context.Wait(MessageReceived);
            return;
        }
        if (tipomossa.Entity == "carta")
        {
            var msg = context.MakeMessage();

            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Human_paper.png", "Hands_Human_paper.png"));
            await context.PostAsync(msg);


        }
        else if (tipomossa.Entity == "forbice")
        {
            var msg = context.MakeMessage();

            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Human_scissors.png", "Hands_Human_scissors.png"));
            await context.PostAsync(msg);

        }
        else
        {
            var msg = context.MakeMessage();

            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Human_rock.png", "Hands_Human_rock.png"));
            await context.PostAsync(msg);

        }

        string moves = "";
        string results = "";
        if (_risultati != null && _risultati.Count > 0)

            foreach (var item in _risultati)
            {
                moves += item.mossa;
                _risultati += item.esito;
            }
        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
        string resultMachine = client.GetStringAsync(String.Format("https://rpscodemotion.azurewebsites.net/api/RPSmove?playerMoves={0}&comMoves={1}&level=0", moves, results);
        resultMachine = resultMachine.Replace("\"", "");

        if (resultMachine == "S")
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_scissors.png", "Hands_Robot_scissors.png"));
        if (resultMachine == "P")
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_paper.png", "Hands_Robot_paper.png"));
        else
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_rock.png", "Hands_Robot_rock.png"));

        await context.PostAsync(msg);

        if (tipomossa.Entity == "carta" && resultMachine == "S")
        {
            _risultati.Add(new risultati() { esito = "0", mossa = "P" });
            roundResultMachine++;
        }
        if (tipomossa.Entity == "carta" && resultMachine == "P")
            _risultati.Add(new risultati() { esito = "0", mossa = "P" });

        if (tipomossa.Entity == "carta" && resultMachine == "R")
        {
            _risultati.Add(new risultati() { esito = "1", mossa = "P" });
            roundResult++;
        }
        if (tipomossa.Entity == "forbice" && resultMachine == "S")
        {
            _risultati.Add(new risultati() { esito = "0", mossa = "S" });
            roundResultMachine++;
        }
        if (tipomossa.Entity == "forbice" && resultMachine == "P")
        {
            _risultati.Add(new risultati() { esito = "1", mossa = "S" });
            roundResult++;
        }
        if (tipomossa.Entity == "forbice" && resultMachine == "R")
        {
            _risultati.Add(new risultati() { esito = "0", mossa = "S" });
            roundResultMachine++;
        }
        if (tipomossa.Entity == "sasso" && resultMachine == "R")
            _risultati.Add(new risultati() { esito = "0", mossa = "R" });

        if (tipomossa.Entity == "sasso" && resultMachine == "S")
        {
            _risultati.Add(new risultati() { esito = "1", mossa = "R" });
            roundResult++;
        }
        if (tipomossa.Entity == "sasso" && resultMachine == "P")
        {
            _risultati.Add(new risultati() { esito = "0", mossa = "R" });
            roundResultMachine++;
        }
        if (roundNumber == 1)
        {
            // da togliere

            var msg = context.MakeMessage();

            roundNumber = 2;
            await context.PostAsync($"Secondo round, fai la tua mossa");

        }
        else if (roundNumber == 2)

        {
            var msg = context.MakeMessage();

            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_paper.png", "Hands_Robot_paper.png"));
            await context.PostAsync(msg);

            roundNumber = 3;
            await context.PostAsync($"Terzo round, fai la tua mossa");

        }
        else
        {

            var newmsg = context.MakeMessage();

            if (roundResult > roundResultMachine)
                newmsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/You_win.png", "You_win.png"));
            if (roundResult < roundResultMachine)
                newmsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/You_lose.png", "You_lose.png"));
            if (roundResult == roundResultMachine)
                newmsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/You_equal.png", "You_equal.png"));
            await context.PostAsync(newmsg);

            roundNumber = 0;
            roundResult = 0;
            roundResultMachine = 0;

            await context.PostAsync($"Partita terminata, digita avvia partita per un nuovo incontro");

        }

        context.Wait(MessageReceived);

        try
        {
            context.UserData.SetValue<System.Collections.Generic.List<risultati>>("risultati", _risultati);
        }
        catch (Exception ex)
        {

            await context.PostAsync($"Errore," + ex.Message);
        }

    }
}