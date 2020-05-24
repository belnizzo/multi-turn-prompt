// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.NumberWithUnit;

using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    public class County
    {
        public County()
        {
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Capital { get; set; }


        [JsonProperty("sub_counties")]
        public string[] SubCounties { get; set; }
    }
    [Newtonsoft.Json.JsonArrayAttribute("sub_counties")]
    public class SubCounty
    {
        public string Name { get; set; }
    }
    public static class Constants
    {
        private static List<County> _testCounties = JsonConvert.DeserializeObject<List<County>>(File.ReadAllText("countiessubcounties.json"));
        public static List<County> TestCounties = _testCounties;
        public static List<string> Counties = new List<string>()
        {
            "Embu","Mombasa","Kwale Coast"," Kilifi Coast","Tana River","Lamu Coast","Taita Taveta","Garissa North Eastern","Wajir North Eastern","Mandera North Eastern","Marsabit Eastern","Isiolo Eastern","Meru Eastern","Tharaka Nithi Eastern ","Embu Eastern", "Kitui Eastern", "Machakos Eastern","Makueni Eastern","Nyandarua Central", "Nyeri Central", "Kirinyaga Central"," Murang'a Central","Kiambu Central"," Turkana Rift Valley","West Pokot Rift Valley",
            "Samburu Rift Valley","Uasin Gishu Rift Valley","Trans-Nzoia Rift Valley","Elgeyo-Marakwet Rift Valley","Nandi Rift Valley","Baringo Rift Valley","Laikipia Rift Valley","Nakuru Rift Valley"," Narok Rift Valley","Kajiado Rift Valley","Kericho Rift Valley","Bomet Rift Valley","Kakamega Western","Vihiga Western","Bungoma Western",
            "Busia Western","Siaya Nyanza","Kisumu Nyanza","Homa Bay Nyanza","Migori Nyanza","Kisii Nyanza",
            "Nyamira Nyanza","Nairobi"
        };

        public static List<string> SubCounties = new List<string>()
        { "Kilifi","Kwale","Lamu","Mombasa","Taita-Taveta","Garissa","Mandera","Embu","Isiolo","Tharaka-Nikthi","Makueni","Kitui","Mutomo",
            "Meru","Kiringyaga","Muranga","Nyandarua","Nyeri","Karbanet","Narok","Laikipia","Kericho","Lodwar","Nandi","Nakuru","Samburu","Kitale",
            "Marala","Kakamega","Kisii"
        };

        public static List<string> Ward = new List<string>()
        {
            "Westlands","Kasarani","Dagoretti","Starehe","Langata","Embakasi","Kamukunji","Njiru","Makadara",
        };
    }

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
                    Prompt = MessageFactory.Text("Hujambo, karibu katika huduma yetu.Tafadhali chagua lugha inayokufaa(1.Kiswahili),(2.Kingereza)." +
                    "  Hello, Welcome to our service.Please choose your preferred langauge(1.Kiswahili), (2.English)"),

                    Choices = ChoiceFactory.ToChoices(new List<string> { "English", "Kiswahili" }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["language"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["language"] == "Kiswahili")
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Tafadhali weka jina Lako") }, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
            }

        }
        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = (string)stepContext.Result;
            if (stepContext.Values["language"] == "Kiswahili")
            {
                // We can send messages to the user at any point in the WaterfallStep.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Asante {stepContext.Result}."), cancellationToken);

                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Ungependa kuongeza ujume?") }, cancellationToken);
            }
            else
            {
                // We can send messages to the user at any point in the WaterfallStep.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thanks {stepContext.Result}."), cancellationToken);

                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to give more details?") }, cancellationToken);

            }

        }


        private async Task<DialogTurnResult> CountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Values["language"] == "Kiswahili")
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Weka Counti unaoishi"),
                    Choices = ChoiceFactory.ToChoices(Constants.Counties),
                }, cancellationToken);

            }
            else
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                  new PromptOptions
                  {
                      Prompt = MessageFactory.Text("Which county do you live in?"),
                      Choices = ChoiceFactory.ToChoices(Constants.Counties),
                  }, cancellationToken);

            }
        }
        private async Task<DialogTurnResult> SubcountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["county"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["language"] == "Kiswahili")

            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Unaishi SabKaunti gani?"),
                    Choices = ChoiceFactory.ToChoices(Constants.SubCounties),
                }, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Which sub-county do you live in?"),
                   Choices = ChoiceFactory.ToChoices(Constants.SubCounties),
               }, cancellationToken);
            }

        }
        private async Task<DialogTurnResult> WardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["subcounty"] = ((FoundChoice)stepContext.Result).Value;
            if (stepContext.Values["language"] == "Kiswahili")
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Unashi wardi gani?"),
                    Choices = ChoiceFactory.ToChoices(Constants.Ward),
                }, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which ward do you live in?"),
                    Choices = ChoiceFactory.ToChoices(Constants.Ward),
                }, cancellationToken);
            }

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

            if (stepContext.Values["language"] == "Kiswahili")
            {
                var msg = $"Hongera!! {userProfile.Name }. Sasa ujume wako umekalimika. Chagua  (1.MAIN MENU)  kuendelea.";

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);


                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);


            }
            else
            {
                var msg = $"Conguratulations {userProfile.Name }. You are now registered to our services.Please choose (1.MAIN MENU) to continue with our service";

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);


                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is the end.
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            }


        }


    }
}
