# AzureTextModerationServices

Start using AI today with Azure Text Moderation Services

The Microsoft Azure Text Moderation API is offered as part of the Azure Cognitive Services and allows uses to easily moderate text that may come in from an outside source, such as a product review, email, or blog comment, in order to ensure there is no offensive content you need to block.

This repository has production ready code that uses these services, and also provides integration tests that are useful for learning how the services work.  All that is necessary is to sign up for the Azure service, then download the code and run the integration tests.

The first step is to sign up for the Azure Content Moderator service.  If you don’t know how to do that check my Youtube channel for instructions or simply go to 
•	Portal.azure.com
•	Create a resource
•	Search for “Content Moderator”
•	Press Create
This will give you the API key you need to use the code here.

Next clone this repo and edit the SetEnvironmentVars.cmd file to add the information about the endpoint you just created.  You’ll have to run this CMD file before starting Visual Studio.

Now you should be able to open the solution in VS2019 (probably 2017 also but I have not tested it).  Build the solution, which should pull down the few NuGet packages necessary, then start running the integration tests.

A good one to start with is the ContentModerationServiceIntgTest.  This integration tests shows examples of how to use the moderation services to submit text for moderation, and how to evaluate the results.  Experiment with the tests to see what Azure considers a bad word and what it doesn’t.

You should also experiment with the Term Lists, which provide a way for the API to search for specific words in the submitted text.



