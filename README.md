# Captions Translator: Empower Global Communication

Welcome to the Captions Translator, an open-source project designed to make translating caption files a breeze with high accuracy. Our mission is to enable seamless communication and accessibility for everyone, regardless of language barriers. By joining our community, you'll be part of a team committed to making a difference in the world.

## Features

- Currently supports YouTube (.srt files) and ChatGPT integration
- Translations available to English
- User-friendly interface for importing and exporting caption files

## We Need Your Help!

While our project has made significant progress, we still have some limitations that we'd love your help to overcome:

- **Expand Supported Platforms:** Currently, our app is limited to YouTube .srt files. Help us integrate other popular platforms and file formats to broaden our reach and impact.
- **Multilingual Support:** As of now, translations are only available to English. Let's work together to provide translations to more languages and foster greater inclusivity.
- **Improve Translation Accuracy:** Contribute to enhancing our translation algorithms and integration with ChatGPT, ensuring our users get the best quality translations possible.

Join our thriving community of developers, designers, and language enthusiasts dedicated to empowering global communication. Your contributions will make a tangible impact on the lives of countless individuals around the world. Get started today by exploring our repository and contributing your unique skills and expertise.

Together, we can make the Captions Translator the go-to tool for global accessibility and communication!

Please follow the (Contributing)[CONTRIBUTING.md] for any contribution.

## How to specify your settings:
This guide provides an explanation on the current method of specifying settings for this project, as well as some considerations for future improvements.

### Environment Variables: Current State
At present, environment variables serve as the method for specifying settings within the project. While this is a functional approach during the development phase, it's important to note that this method does not embody the highest degree of security. 
We aspire to enhance this configuration method by potentially integrating services like 1Password or LastPass in the future.

Suggestions to improve this configuration method are always welcome. Please share your insights and ideas with the team.

> **Note:** For development purposes, settings can be specified in the `appsettings.private.json` or `appsettings.json` files.


### Required Settings
To run the application, the following settings are necessary:

#### Application-Specific Settings

- `TranslationSettings:OriginalFolder`: Specifies the directory on your machine where YouTube files will be downloaded.
- `TranslationSettings:PlainTranslation`: Indicates an intermediate folder containing the progress of the currently translating file.
- `TranslationSettings:TranslationFolder`: Represents the folder where files will be stored post-translation.

#### OpenAI Settings

- `OpenAiSettings:API_KEY`: Your API key from OpenAI. [Find it here](https://help.openai.com/en/articles/4936850-where-do-i-find-my-secret-api-key).
- `OpenAiSettings:Model`: Defines the GPT model to use. Supported options include `3.5turbo` and `4`. Be sure to understand the [pricing implications](https://openai.com/pricing#language-models) for each.

> The current estimated cost for translating a 10-minute video with ChatGPT-4 is approximately $1.5 USD. This figure is derived from our internal experiences and is subject to variation depending on specific project factors.

#### YouTube Settings

Given that this application uploads files and modifies localization translation (titles and descriptions) on YouTube, OAuth2 authentication is necessary. If you're uncomfortable with this, you may choose to manually download the files from YouTube, place them in the `TranslationSettings:OriginalFolder`, and manually upload them once translation is complete.

- `YouTubeSettings:OAuth2ClientId`: ClientId provided by Google.
- `YouTubeSettings:OAuth2ClientSecret`: Secret provided by Google. 

> Here's a helpful guide on [how to obtain an OAuth2Client/Secret from Google](https://developers.google.com/fit/android/get-api-key).

## Frontend Selection: Blazor in a MAUI App

Our frontend is powered by Blazor, which operates within a .NET Multi-platform App UI (MAUI) App, utilizing WebAssembly (WASM). This choice was influenced by several factors, as detailed below.

### Justifying the Choice

While other frameworks like [Uno Platform](https://platform.uno/) or [Avalonia UI](https://avaloniaui.net/) are viable options for creating desktop applications, they were deemed unsuitable for this Proof of Concept (PoC) due to the learning curve involved. 

Conversely, leveraging our existing proficiency in Blazor resulted in a more efficient development process. This selection also offers the advantage of being a "browser" app, thus providing excellent adaptability to various resolutions and easy integration of custom CSS.

### Language Compatibility: C#

The selected frontend technology needs to be C# compatible as our overarching aim is to bundle the entire app together. Given that the domain logic is written in C#, a C# compatible frontend is a requirement.

We are open to exploring and adopting other frontend technologies, so long as they meet this C# compatibility criterion.


## Support

### ⭐️ Star Us

If you found this project useful, you could support us by giving it a star! 🌟

[![GitHub stars](https://img.shields.io/github/stars/ElectNewt/CaptionsTranslator?style=social)](https://github.com/ElectNewt/CaptionsTranslator/stargazers)

Simply click [here](https://github.com/ElectNewt/CaptionsTranslator/stargazers) to navigate to the repository and click on the "Star" button at the top right corner.

### ☕ Buy Me a Coffee

You can support this project and [keep me caffeinated](https://www.buymeacoffee.com/NetMentor) by making a donation. Every cup of coffee is greatly appreciated and will be used to keep me productive!

[![Buy me a coffee](https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png)](https://www.buymeacoffee.com/NetMentor)

