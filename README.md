**🛈 This repository is intended as a read-only source of information, and contributions by the general public are not expected.**

# LearningKit

 LearningKit is a functional website for learning purposes. It demonstrates how to implement various Kentico Xperience features on MVC websites in the form of code snippets, which you can run if you connect the website to a Kentico Xperience database.

## Instructions for running the LearningKit project

1. [Install](https://docs.xperience.io/x/bAmRBg) Kentico Xperience in the <span>ASP.</span>NET Core development model. When choosing the installation type, select *New site*.
1. Set the **Presentation URL** of the new site to the URL where you plan to run the LearningKit project.
1. [Enable web farms](https://docs.xperience.io/x/Mw_RBg) in automatic mode.
1. Rename the `LearningKitCore\appsettings.json.template` file to `appsettings.json`.
1. Copy the `CMSConnectionString` connection string from the Xperience administration application's `web.config` file to the `LearningKitCore\appsettings.json` file.

    ``` json
    "ConnectionStrings": {
        "CMSConnectionString": "<YourConnectionString>"
    }
    ```

1. Copy the `CMSHashStringSalt` app setting from the Xperience administration application's `web.config` file to the `LearningKitCore\appsettings.json` file.

    ``` json
    "CMSHashStringSalt": "<AdministrationHashStringSalt>",
    ```

1. Open the `LearningKitCore.sln` solution in Visual Studio and run the LearningKit web application.

## Accessing older versions of the repository

You can find older versions of the LearningKit project under the [Releases section](https://github.com/KenticoInternal/LearningKit-Mvc/releases) (open the corresponding commit for the required version and click **Browse files**).

Quick links:

- [LearningKit for Kentico 12 Service Pack](https://github.com/KenticoInternal/LearningKit-Mvc/releases/tag/2.0.0) (for A<span>SP.</span>NET MVC 5)
- [LearningKit for Kentico 12](https://github.com/KenticoInternal/LearningKit-Mvc/releases/tag/v1.0.0) (for A<span>SP.</span>NET MVC 5)
