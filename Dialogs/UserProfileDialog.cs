﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class UserProfileDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public UserProfileDialog(UserState userState)
            : base(nameof(UserProfileDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                LanguageStepAsync,
                NameStepAsync,
                NameConfirmStepAsync,
                CountyStepAsync,
                SubcountyStepAsync,
                WardStepAsync,              
                SummaryStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
         

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> LanguageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the user's response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Hello, Welcome to our service.Please choose your preferred Langauage"),
                    Choices = ChoiceFactory.ToChoices(new List<string> {"English", "Kisawahili"}),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["language"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
        }
        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = (string)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to give more details?") }, cancellationToken);
        }


        private async Task<DialogTurnResult> CountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
          
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Which county do you live in?"),
                   Choices = ChoiceFactory.ToChoices(new List<string> { "Nairobi", "Kisumu" ,"Trans-Nzoia"}),
               }, cancellationToken);

        }
        private async Task<DialogTurnResult> SubcountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["county"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Which sub - county do you live in?"),
                   Choices = ChoiceFactory.ToChoices(new List<string> { "Kilifi", "Kilifi)",
"Kwale District ","Lamu District","Mombasa","Wundanyi","Hola","Garissa", "mandera","Wanjir"," Embu"



//Embu District (Embu)
//Isiolo District (Isiolo)
//Kitui District (Kitui)
//Machakos District (Machakos)
//Makueni District (Makueni)
//Marsabit District (Marsabit)
//Meru District (Meru)
//Mutomo District (Mutomo)
//Tharaka-Nithi District (Chuka)
//Central Province:

//Kiambu District (Kiambu)
//Kirinyaga District (Kerugoya/Kutus)
//Murang'a District (Murang'a)
//Nyandarua District(Nyahururu)
//Nyeri District(Nyeri)
//Rift Valley Province:

//Baringo District(Kabarnet)
//Bomet District(Bomet)
//Elgeyo - Marakwet District
//Kajiado District(Kajiado)
//Kericho District(Kericho)
//Laikipia District(Nanyuki)
//Nakuru District(Nakuru)
//Nandi District(Kapsabet)
//Narok District(Narok)
//Samburu District(Maralal)
//Trans Nzoia District(Kitale)
//Turkana District(Lodwar)
//Uasin Gishu District(Eldoret)
//West Pokot District(Kapenguria)
//Western Province:

//Bungoma District(Bungoma)
//Busia(Busia)
//Kakamega District(Kakamega)
//Vihiga District(Vihiga)
//Nyanza Province:

//Homa Bay District(Homa Bay)
//Kisii Central(Kisii)
//Kisumu District(Kisumu)
//Migori District(Migori)
//Nyamira District(Nyamira)
//Siaya District(Siaya)
               }),
               }, cancellationToken);
        }

        
        private async Task<DialogTurnResult> WardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["subcounty"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Which ward do you live in?"),
                   Choices = ChoiceFactory.ToChoices(new List<string> { "Nairobi", "Kisumu", "Trans-Nzoia" }),
               }, cancellationToken);

        }


        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["ward"] = ((FoundChoice)stepContext.Result).Value;

            // Get the current profile object from user state.
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
         
            userProfile.Name = (string)stepContext.Values["name"];
            userProfile.Language = (string)stepContext.Values["language"];
            userProfile.County = (string)stepContext.Values["county"];
            userProfile.Subcounty = (string)stepContext.Values["subcounty"];
            userProfile.Ward = (string)stepContext.Values["ward"];

            var msg = $"Thank you {userProfile.Name } please have a look at the information provided.  Language Selection: {userProfile.Language}      Your county: {userProfile.County}    Your Subcounty: {userProfile.Subcounty}     Your Ward: {userProfile.Ward}. ";
             
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);         
         

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            
        }
    
    }
}
