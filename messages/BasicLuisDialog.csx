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

        context.PostAsync($"Non ho capito cosa hai detto. Hai detto : {result.Query}"); //
        context.Wait(MessageReceived);

    }

    private async Task AfterChoice(IDialogContext context, IAwaitable<string> result)
    {
        var mossa = await result;
        await context.PostAsync("Hai lanciato " + mossa);
        context.Wait(MessageReceived);
    }



    [LuisIntent("Reset")]
    public async Task Reset(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Reset");
        telemetry.Flush();
        var _noresult = new System.Collections.Generic.List<HistoryMove>();

        try
        {
            context.UserData.SetValue<System.Collections.Generic.List<HistoryMove>>("historymymoves", _noresult);

            await context.PostAsync($"Ho azzerato i tuoi dati ");

            context.Wait(MessageReceived);
        }
        catch (Exception ex)
        {
            telemetry = new TelemetryClient();
            telemetry.TrackException(ex);
            telemetry.Flush();
            // await context.PostAsync($"Errore durante il reset ," + ex.Message);
        }
    }
    [LuisIntent("Score")]
    public async Task Score(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Score");
        telemetry.Flush();
        ScoreFight yourScore = null;
        try
        {
            yourScore = context.UserData.Get<ScoreFight>("fightscore");

        }
        catch (Exception ex)
        {
            telemetry = new TelemetryClient();
            telemetry.TrackException(ex);
            telemetry.Flush();

        }

        if (yourScore != null && (yourScore.loseNumber != 0 || yourScore.winNumber != 0 || yourScore.equalNumber != 0))
        {
            await context.PostAsync($"Ecco i tuoi risultati " + yourScore.winNumber.ToString() + " vittorie, " + yourScore.loseNumber.ToString() + " sconfitte, " + yourScore.equalNumber.ToString() + " pareggi!");
        }
        else

            await context.PostAsync($"Nessun dato presente");

        context.Wait(MessageReceived);
    }
    [LuisIntent("Insult")]
    public async Task Insult(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Insult");
        telemetry.Flush();

        await context.PostAsync($"Non insultami , sono tuo amico :)");

        context.Wait(MessageReceived);

    }
    [LuisIntent("Hate")]
    public async Task Hate(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Hate");
        telemetry.Flush();

        await context.PostAsync($"Provo solo provare amore per te <3 ");

        context.Wait(MessageReceived);

    }
    [LuisIntent("Love")]
    public async Task Love(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Love");
        telemetry.Flush();
        await context.PostAsync($"Che dolcetto che sei, strapazzimi di coccole <3");

        context.Wait(MessageReceived);

    }
    [LuisIntent("Welcome")]
    public async Task Welcome(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Welcome Game");
        telemetry.Flush();

        try
        {



            try
            {
                System.Collections.Generic.List<HistoryMove> temp = context.UserData.Get<System.Collections.Generic.List<HistoryMove>>("historymymoves");

                if (temp != null)

                    _hystoryMoves = temp;
            }
            catch (Exception ex)
            {
                // await context.PostAsync($"Errore," + ex.Message);

                telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                telemetry.Flush();
            }


            try
            {
                ScoreFight scoreTemp = context.UserData.Get<ScoreFight>("fightscore");

                if (scoreTemp != null)

                    yourScore = scoreTemp;
            }
            catch (Exception ex)
            {
                //  await context.PostAsync($"Errore," + ex.Message);
                telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
                telemetry.Flush();
            }

            var msg = context.MakeMessage();
            msg.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/paper.png", "paper.png"));
            await context.PostAsync(msg);

            if (!String.IsNullOrEmpty(name))
                await context.PostAsync($"Buongiorno, " + this.name + ", gioca e sfida la nostra intelligenza artificiale al gioco sasso carta e forbice");
            else
                await context.PostAsync($"Buongiorno,  gioca e sfida la nostra intelligenza artificiale al gioco sasso carta e forbice");


            //if (_hystoryMoves != null && _hystoryMoves.Count > 0)
            //{
            //    string moves = "";


            //    for (int i = 0; i < _hystoryMoves.Count; i++)
            //    {
            //        if (i == _hystoryMoves.Count - 1)
            //            moves += (i + 1).ToString() + " ) tu : " + _hystoryMoves[i].human.ToString() + " - macchina : " + _hystoryMoves[i].machine.ToString();
            //        else
            //            moves += (i + 1).ToString() + " ) tu : " + _hystoryMoves[i].human.ToString() + " - macchina : " + _hystoryMoves[i].machine.ToString() + " - ";
            //    }

            //    await context.PostAsync($"Ecco i tuoi ultimi combattimenti : " + moves);
            //}

            if (yourScore != null && (yourScore.loseNumber != 0 || yourScore.winNumber != 0 || yourScore.equalNumber != 0))
            {
                await context.PostAsync($"Intanto che ci pensi...ecco i tuoi risultati " + yourScore.winNumber.ToString() + " vittorie, " + yourScore.loseNumber.ToString() + " sconfitte, " + yourScore.equalNumber.ToString() + " pareggi!");
            }


        }
        catch (Exception ex)
        {
            telemetry = new TelemetryClient();
            telemetry.TrackException(ex);
            telemetry.Flush();
        }
        context.Wait(MessageReceived);

    }
    [LuisIntent("Break")]
    public async Task Break(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Break");
        telemetry.Flush();

        if (roundNumber > 0)
            await context.PostAsync($"Partita interotta, per iniziarne una nuova partita scrivi avvia partita"); //
        else
            await context.PostAsync($"Non hai in corso nessuna partita, per iniziarne una scrivi avvia partita"); //
        roundNumber = 0;
        roundResult = 0;
        roundResultMachine = 0;
        startFight = false;
        context.Wait(MessageReceived);
    }
    [LuisIntent("Regole")]
    public async Task Regole(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Regole");
        telemetry.Flush();
        await context.PostAsync($"Le regole sono semplici, la carta vince sul sasso, la forbice vince su carta e il sasso vince sulle forbici, combatti contro la nostra macchina, per iniziare scrivi partita"); //
        context.Wait(MessageReceived);
    }
    [LuisIntent("Partita")]
    public async Task Partita(IDialogContext context, LuisResult result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Partita");
        telemetry.Flush();
        await context.PostAsync($"Iniziamo la partita che è composta da 5 round, vince chi tra te e la nostra macchina si aggiudica il numero maggiore. Inizia scrivendo avvia partita"); //
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
        roundResult = 0;
        roundResultMachine = 0;
        //await context.PostAsync($"Primo round, scrivi la tua mossa ( sasso , carta o forbice ) ");
        //context.Wait(MessageReceived);

        var options = new PromptOptions<string>("Fai la tua mossa o termina la partita", null, "Devi selezionare una mossa o termina la partita", new List<string>() { "sasso", "carta", "forbice", "termina" }, 3, new PromptStyler(PromptStyle.Auto));
        PromptDialog.Choice<string>(context, FaiLaTuaMossa, options);
    }
    [LuisIntent("Mossa")]
    public async Task Mossa(IDialogContext context, LuisResult result)
    {
        if (!startFight)
        {
            startFight = true;
            roundNumber = 1;
            roundResult = 0;
            roundResultMachine = 0;
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
        if (tipomossa.Entity.ToLower() == "carta")
        {
            var msg = context.MakeMessage();

            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Human_paper.png", "Hands_Human_paper.png"));
            await context.PostAsync(msg);


        }
        else if (tipomossa.Entity.ToLower() == "forbice")
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

            telemetry = new TelemetryClient();
            telemetry.TrackException(ex);
            telemetry.Flush();
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
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "P", machine = "S" });
            roundResultMachine++;
        }
        if (tipomossa.Entity == "carta" && resultMachine == "P")

            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "P", machine = "P" });

        if (tipomossa.Entity == "carta" && resultMachine == "R")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "P", machine = "R" });
            roundResult++;
        }
        if (tipomossa.Entity == "forbice" && resultMachine == "S")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "S", machine = "S" });

        }
        if (tipomossa.Entity == "forbice" && resultMachine == "P")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "S", machine = "P" });
            roundResult++;
        }
        if (tipomossa.Entity == "forbice" && resultMachine == "R")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "S", machine = "R" });
            roundResultMachine++;
        }
        if (tipomossa.Entity == "sasso" && resultMachine == "R")

            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "R", machine = "R" });

        if (tipomossa.Entity == "sasso" && resultMachine == "S")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "R", machine = "S" });
            roundResult++;
        }
        if (tipomossa.Entity == "sasso" && resultMachine == "P")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "R", machine = "P" });
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
        else if (roundNumber == 3)
        {
            roundNumber = 4;
            await context.PostAsync($"Quarto round, fai la tua mossa");
        }
        else if (roundNumber == 4)
        {
            roundNumber = 5;
            await context.PostAsync($"Ultimo round, fai la tua mossa");
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

                if (fightResult == 2)
                    yourScore.equalNumber++;

                context.UserData.SetValue<ScoreFight>("fightscore", yourScore);

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
            telemetry = new TelemetryClient();
            telemetry.TrackException(ex);
            telemetry.Flush();
            await context.PostAsync($"Errore," + ex.Message);
        }

        stopwatch.Stop();

        telemetry.TrackRequest("Mossa", DateTime.Now,
           stopwatch.Elapsed,
           "200", true);
        telemetry.Flush();
    }


    public async Task RiavviaPartita(IDialogContext context, IAwaitable<string> result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("Riavvia Partita");
        telemetry.Flush();
        string tipomossa = await result;
        if (tipomossa == "" || tipomossa == "basta")
        {
            await context.PostAsync($"Grazie per aver giocato, per una nuova partita digita avvia partita oppure visualizza il tuo punteggio digitando risultati"); //
            context.Wait(MessageReceived);
            return;
        }

        startFight = true;
        roundNumber = 1;

        var options = new PromptOptions<string>("Fai la tua mossa o termina la partita", null, "Devi selezionare una mossa o termina la partita", new List<string>() { "sasso", "carta", "forbice", "termina" }, 3, new PromptStyler(PromptStyle.Auto));
        PromptDialog.Choice<string>(context, FaiLaTuaMossa, options);
    }
    public async Task FaiLaTuaMossa(IDialogContext context, IAwaitable<string> result)
    {
        TelemetryClient telemetry = new TelemetryClient();
        telemetry.TrackEvent("FaiLaTuaMossa");
        telemetry.Flush();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        string tipomossa = await result;

        if (tipomossa == "" || tipomossa == "termina")
        {
            await context.PostAsync($"Partita terminata, per una nuova partita digita avvia partita"); //
            context.Wait(MessageReceived);
            return;
        }
        if (tipomossa.ToLower() == "carta")
        {
            var msg = context.MakeMessage();
            msg.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Human_paper.png", "Hands_Human_paper.png"));
            context.PostAsync(msg);


        }
        else if (tipomossa.ToLower() == "forbice")
        {
            var msg = context.MakeMessage();
            msg.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Human_scissors.png", "Hands_Human_scissors.png"));
            context.PostAsync(msg);

        }
        else
        {
            var msg = context.MakeMessage();
            msg.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
            msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Human_rock.png", "Hands_Human_rock.png"));
            context.PostAsync(msg);

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
            context.PostAsync($"Errore nell'invocazione di AI " + ex.Message);

            telemetry = new TelemetryClient();
            telemetry.TrackException(ex);
            telemetry.Flush();
        }

        var machineMsg = context.MakeMessage();
        machineMsg.Attachments = new List<Microsoft.Bot.Connector.Attachment>();

        if (resultMachine == "S")
            machineMsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_scissors.png", "Hands_Robot_scissors.png"));
        else if (resultMachine == "P")
            machineMsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_paper.png", "Hands_Robot_paper.png"));
        else if (resultMachine == "R")
            machineMsg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://fifthelementstorage.blob.core.windows.net/bot/Hands_Robot_rock.png", "Hands_Robot_rock.png"));
        else
            context.PostAsync($"Non ho nessuna mossa : " + resultMachine);

        context.PostAsync(machineMsg);

        if (tipomossa.ToLower() == "carta" && resultMachine == "S")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "P", machine = "S" });
            roundResultMachine++;
        }
        if (tipomossa.ToLower() == "carta" && resultMachine == "P")

            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "P", machine = "P" });

        if (tipomossa.ToLower() == "carta" && resultMachine == "R")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "P", machine = "R" });
            roundResult++;
        }
        if (tipomossa.ToLower() == "forbice" && resultMachine == "S")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "S", machine = "S" });

        }
        if (tipomossa.ToLower() == "forbice" && resultMachine == "P")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "S", machine = "P" });
            roundResult++;
        }
        if (tipomossa.ToLower() == "forbice" && resultMachine == "R")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "S", machine = "R" });
            roundResultMachine++;
        }
        if (tipomossa.ToLower() == "sasso" && resultMachine == "R")

            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "R", machine = "R" });

        if (tipomossa.ToLower() == "sasso" && resultMachine == "S")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "1", human = "R", machine = "S" });
            roundResult++;
        }
        if (tipomossa.ToLower() == "sasso" && resultMachine == "P")
        {
            _hystoryMoves.Add(new HistoryMove() { result = "0", human = "R", machine = "P" });
            roundResultMachine++;
        }
        if (roundNumber == 1)
        {

            roundNumber = 2;
            var options = new PromptOptions<string>("Parziale " + roundResult + " a " + roundResultMachine + " ! Secondo round, fai la tua mossa o termina la partita", null, "Deve selezionare una mossa", new List<string>() { "sasso", "carta", "forbice", "termina" }, 3, new PromptStyler(PromptStyle.Auto));
            PromptDialog.Choice<string>(context, FaiLaTuaMossa, options);
            //   await context.PostAsync($"Secondo round, fai la tua mossa");
        }
        else if (roundNumber == 2)
        {
            roundNumber = 3;
            //  await context.PostAsync($"Terzo round, fai la tua mossa");
            var options = new PromptOptions<string>("Parziale " + roundResult + " a " + roundResultMachine + " ! Terzo round, fai la tua mossa o termina la partita", null, "Deve selezionare una mossa", new List<string>() { "sasso", "carta", "forbice", "termina" }, 3, new PromptStyler(PromptStyle.Auto));
            PromptDialog.Choice<string>(context, FaiLaTuaMossa, options);
        }
        else if (roundNumber == 3)
        {
            roundNumber = 4;
            //await context.PostAsync($"Quarto round, fai la tua mossa");
            var options = new PromptOptions<string>("Parziale " + roundResult + " a " + roundResultMachine + " ! Quarto round, fai la tua mossa o termina la partita", null, "Deve selezionare una mossa", new List<string>() { "sasso", "carta", "forbice", "termina" }, 3, new PromptStyler(PromptStyle.Auto));
            PromptDialog.Choice<string>(context, FaiLaTuaMossa, options);
        }
        else if (roundNumber == 4)
        {
            roundNumber = 5;
            //await context.PostAsync($"Ultimo round, fai la tua mossa");
            var options = new PromptOptions<string>("Parziale " + roundResult + " a " + roundResultMachine + " ! Ultimo round, fai la tua mossa o termina la partita", null, "Deve selezionare una mossa", new List<string>() { "sasso", "carta", "forbice", "termina" }, 3, new PromptStyler(PromptStyle.Auto));
            PromptDialog.Choice<string>(context, FaiLaTuaMossa, options);
        }
        else
        {
            var newmsg = context.MakeMessage();
            int fightResult = 0;
            newmsg.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
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

                if (fightResult == 2)
                    yourScore.equalNumber++;

                context.UserData.SetValue<ScoreFight>("fightscore", yourScore);

            }
            catch (Exception ex)
            {

                await context.PostAsync($"Errore," + ex.Message);
            }

            roundNumber = 0;
            roundResult = 0;
            roundResultMachine = 0;
            startFight = false;

            //await context.PostAsync($"Partita terminata, digita avvia partita per un nuovo incontro");

            //context.Wait(MessageReceived);

            var options = new PromptOptions<string>("Partita terminata, vuoi fare una nuova partita o basta?", null, "Deve selezionare una mossa", new List<string>() { "nuova partita", "basta" }, 3, new PromptStyler(PromptStyle.Auto));
            PromptDialog.Choice<string>(context, RiavviaPartita, options);
        }


        try
        {
            context.UserData.SetValue<System.Collections.Generic.List<HistoryMove>>("historymymoves", _hystoryMoves);
        }
        catch (Exception ex)
        {
            telemetry = new TelemetryClient();
            telemetry.TrackException(ex);
            telemetry.Flush();
            await context.PostAsync($"Errore," + ex.Message);
        }

        stopwatch.Stop();

        telemetry.TrackRequest("Mossa", DateTime.Now,
           stopwatch.Elapsed,
           "200", true);
        telemetry.Flush();
    }
}