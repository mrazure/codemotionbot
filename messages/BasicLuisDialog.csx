using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.ApplicationInsights;

[Serializable]
public class HistoryMove
{

    public string human { get; set; }

    public string machine { get; set; }

    public string result { get; set; }
}
[Serializable]
public class ScoreFight
{
    public int winNumber { get; set; }

    public int loseNumber { get; set; }

    public int equalNumber { get; set; }
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
    bool startFight = false;
    ScoreFight yourScore = new ScoreFight();

    System.Collections.Generic.List<HistoryMove> _hystoryMoves = new System.Collections.Generic.List<HistoryMove>();
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

    [LuisIntent("Reset")]
    public async Task Reset(IDialogContext context, LuisResult result)
    {
        var _noresult = new System.Collections.Generic.List<HistoryMove>();

        try
        {
            context.UserData.SetValue<System.Collections.Generic.List<HistoryMove>>("historymymoves", _noresult);

            await context.PostAsync($"Ho azzerato i tuoi dati ");

            context.Wait(MessageReceived);
        }
        catch (Exception ex)
        {

            await context.PostAsync($"Errore durante il reset ," + ex.Message);
        }
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
                System.Collections.Generic.List<HistoryMove> temp = context.UserData.Get<System.Collections.Generic.List<HistoryMove>>("historymymoves");

                if (temp != null)

                    _hystoryMoves = temp;
            }
            catch (Exception ex)
            {
                // await context.PostAsync($"Errore," + ex.Message);

            }


            try
            {
                ScoreFight scoreTemp = context.UserData.Get<ScoreFight>("yourscore");

                if (scoreTemp != null)

                    yourScore = scoreTemp;
            }
            catch (Exception ex)
            {
                // await context.PostAsync($"Errore," + ex.Message);

            }

            var msg = context.MakeMessage();
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/paper.png", "paper.png"));
            await context.PostAsync(msg);

            if (!String.IsNullOrEmpty(name))
                await context.PostAsync($"Buongiorno," + this.name + ",  hai a disposizione due comandi regole e partita"); //
            else
                await context.PostAsync($"Buongiorno, hai a disposizione due comandi regole e partita"); //


            if (_hystoryMoves != null && _hystoryMoves.Count > 0)
            {
                string moves = "";


                for (int i = 0; i < _hystoryMoves.Count; i++)
                {
                    if (i == _hystoryMoves.Count - 1)
                        moves += (i + 1).ToString() + " ) tu : " + _hystoryMoves[i].human.ToString() + " - macchina : " + _hystoryMoves[i].machine.ToString();
                    else
                        moves += (i + 1).ToString() + " ) tu : " + _hystoryMoves[i].human.ToString() + " - macchina : " + _hystoryMoves[i].machine.ToString() + " - ";
                }

                await context.PostAsync($"Ecco i tuoi ultimi combattimenti : " + moves);
            }

            if (yourScore != null && (yourScore.loseNumber != 0 || yourScore.winNumber != 0 || yourScore.equalNumber))
            {
                await context.PostAsync($"Ecco i tuoi risultati " + yourScore.winNumber.ToString() + " vittorie, " + yourScore.loseNumber.ToString() + " sconfitte, " + yourScore.equalNumber.ToString() + " pareggi!");
            }


        }
        catch (Exception)
        {


        }


        context.Wait(MessageReceived);



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
        startFight = true;
        roundNumber = 1;
        await context.PostAsync($"Primo round, lancia la tua mossa ( sasso , carta o forbice ) ");
        context.Wait(MessageReceived);
    }
    [LuisIntent("Mossa")]
    public async Task Mossa(IDialogContext context, LuisResult result)
    {
        if (!startFight)
        {
            startFight = true;
            roundNumber = 1;
            await context.PostAsync($"Ok avvio una partita, primo round, lancia la tua mossa ( sasso , carta o forbice ) ");
            context.Wait(MessageReceived);
            return;
        }
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Mossa");
        telemetry.Flush();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
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

        string humanMoves = "";
        string machineMoves = "";
        string resultMachine = "";

        try
        {
            if (_hystoryMoves != null && _hystoryMoves.Count > 0)

                foreach (var item in _hystoryMoves)
                {
                    humanMoves += item.human;
                    machineMoves += item.machine;
                }

            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

            resultMachine = await client.GetStringAsync(String.Format("https://codemotionmilano.azurewebsites.net/api/FunctionCodeMotionMilano?playerMoves={0}&comMoves={1}&level=0", humanMoves, machineMoves));

            resultMachine = resultMachine.Replace("\"", "");

        }
        catch (Exception ex)
        {
            await context.PostAsync($"Errore nell'invocazione di AI " + ex.Message);

        }

        var machineMsg = context.MakeMessage();

        if (resultMachine == "S")
            machineMsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_scissors.png", "Hands_Robot_scissors.png"));
        else if (resultMachine == "P")
            machineMsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_paper.png", "Hands_Robot_paper.png"));
        else if (resultMachine == "R")
            machineMsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_rock.png", "Hands_Robot_rock.png"));
        else
            await context.PostAsync($"Non ho nessuna mossa : " + resultMachine);

        await context.PostAsync(machineMsg);

        if (tipomossa.Entity == "carta" && resultMachine == "S")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "P", machine = "P" });
            roundResultMachine++;
        }
        if (tipomossa.Entity == "carta" && resultMachine == "P")

            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "P", machine = "P" });

        if (tipomossa.Entity == "carta" && resultMachine == "R")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "P", machine = "P" });
            roundResult++;
        }
        if (tipomossa.Entity == "forbice" && resultMachine == "S")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "S", machine = "S" });
            roundResultMachine++;
        }
        if (tipomossa.Entity == "forbice" && resultMachine == "P")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "S", machine = "S" });
            roundResult++;
        }
        if (tipomossa.Entity == "forbice" && resultMachine == "R")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "S", machine = "S" });
            roundResultMachine++;
        }
        if (tipomossa.Entity == "sasso" && resultMachine == "R")

            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "R", machine = "R" });

        if (tipomossa.Entity == "sasso" && resultMachine == "S")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "R", machine = "R" });
            roundResult++;
        }
        if (tipomossa.Entity == "sasso" && resultMachine == "P")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "R", machine = "R" });
            roundResultMachine++;
        }
        if (roundNumber == 1)
        {

            roundNumber = 2;
            await context.PostAsync($"Secondo round, fai la tua mossa");
        }
        else if (roundNumber == 2)
        {
            roundNumber = 3;
            await context.PostAsync($"Terzo round, fai la tua mossa");
        }
        else
        {
            var newmsg = context.MakeMessage();
            int fightResult = 0;

            if (roundResult > roundResultMachine)
            {
                newmsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/You_win.png", "You_win.png"));
                fightResult = 1;
            }
            if (roundResult < roundResultMachine)
            {
                newmsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/You_lose.png", "You_lose.png"));
                fightResult = 0;
            }
            if (roundResult == roundResultMachine)
            {
                fightResult = 2;
                newmsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/You_equal.png", "You_equal.png"));
            }
            await context.PostAsync(newmsg);

            try
            {
                if (yourScore == null)

                    yourScore = new ScoreFight();

                if (fightResult == 0)
                    yourScore.loseNumber++;

                if (fightResult == 1)
                    yourScore.winNumber++;

                if (fightResult == 0)
                    yourScore.equalNumber++;

                context.UserData.SetValue<ScoreFight>("yourscore", yourScore);

            }
            catch (Exception ex)
            {

                await context.PostAsync($"Errore," + ex.Message);
            }

            roundNumber = 0;
            roundResult = 0;
            roundResultMachine = 0;
            startFight = false;

            await context.PostAsync($"Partita terminata, digita avvia partita per un nuovo incontro");

        }

        context.Wait(MessageReceived);

        try
        {
            context.UserData.SetValue<System.Collections.Generic.List<HistoryMove>>("historymymoves", _hystoryMoves);
        }
        catch (Exception ex)
        {

            await context.PostAsync($"Errore," + ex.Message);
        }

        stopwatch.Stop();

        telemetry.TrackRequest("Mossa", DateTime.Now,
           stopwatch.Elapsed,
           "200", true);
        telemetry.Flush();
    }
}