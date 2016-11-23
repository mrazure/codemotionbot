using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
 

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
  int roundNumber = 0;
    
    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {
    }

    [LuisIntent("")]
    public async Task None(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Non ho capito cosa hai detto. Hai detto : {result.Query}"); //
        context.Wait(MessageReceived);
    }

    
    [LuisIntent("Welcome")]
    public async Task Welcome(IDialogContext context, LuisResult result)
    {
        
          
        var msg = context.MakeMessage();
        // msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "http://rockpaperscissors.mybluemix.net/img/Background_Scissors.png","Background_Scissors.png"));
        msg.Attachments.Add(new Microsoft.Bot.Connector.Attachment("image/png", "https://codemotionbot.azurewebsites.net/images/paper.png", "paper.png"));
        await context.PostAsync(msg);
        await context.PostAsync($"Buongiorno, hai a disposizione due comandi regole e partita"); //
        context.Wait(MessageReceived);
        
    }
    [LuisIntent("Regole")]
    public async Task Regole(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Le regole sono semplici, la carta vince sul sasso, la forbice vince su carta e il sasso vince sulle forbici, per iniziare scrivi partita"); //
        context.Wait(MessageReceived);
    }
     [LuisIntent("Partita")]
    public async Task Partita(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"Iniziamo la partita che è composta da 3 round, vince chi se ne aggiudica 2. Inizia scrivendo avvia partita"); //
        context.Wait(MessageReceived);
    }
    [LuisIntent("AvviaPartita")]
    public async Task AvviaPartita(IDialogContext context, LuisResult result)
    {

         roundNumber = 1;
        await context.PostAsync($"Primo round, fai la tua mossa");
        context.Wait(MessageReceived);
    }
     [LuisIntent("Mossa")]
    public async Task Mossa(IDialogContext context, LuisResult result)
    {
        
       
        if (roundNumber  == 1)
        {
              roundNumber = 2;
           await context.PostAsync($"Secondo round, fai la tua mossa"); 
           
        }
        else if(  roundNumber == 2)
       
        {
             roundNumber = 3;
           await context.PostAsync($"Terzo round, fai la tua mossa"); 
             
        }
        else
        {
                 roundNumber = 0;
           await context.PostAsync($"Partita terminata, digita avvia partita per un nuovo incontro"); 
        
            
        }
        
          context.Wait(MessageReceived);
    }
}