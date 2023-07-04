# OpenAI ChatGPT Plugin - NET Pets Sample

_Create, customize and deploy your own ChatGPT PlugIn in minutes._ ✨

This is a quickstart for sample for creating [ChatGPT Plugin](https://openai.com/blog/chatgpt-plugins) programing in NET, using GitHub Codespaces, VS Code, and Azure. 

To gain access to ChatGPT plugins, [join waitlist here](https://openai.com/waitlist/plugins)!

## Getting started (coming soon!)

1. **📤 One-click setup**: [Open a new Codespace](https://codespaces.new/azure-samples/openai-plugin-fastapi), giving you a fully configured cloud developer environment.
2. **🪄 Make an API**: Add routes in `src/OpenAI.PlugIn.NETSample/Program.cs`. Check [Route Handlers in Minimal API apps](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/route-handlers?view=aspnetcore-7.0) for more information.
3. **▶️ Run, one-click again**: Use VS Code's built-in *Run* command and open the forwarded port *8000* in your browser.
4. **💬 Test in ChatGPT**: Copy the URL (make sure its public) and paste it in ChatGPT's [Develop your own plugin](https://platform.openai.com/docs/plugins/getting-started/debugging) flow.
5. **🔄 Iterate quickly:** Codespaces updates the server on each save, and VS Code's debugger lets you dig into the code execution.

## Run Local
1. Open `src/OpenAI.PlugIn.NETSample/Program.cs`. Press [F5] To Start Debugging. Choose your prefered Debugger.
1. Once the project is compiled, the app should be running. A new browser will open with the URL [http://localhost:5100/swagger/index.html].
    ![Run and Debug App](/images/01RunandDebugApp.png "Run and Debug App")

1. Open ChatGPT. Select `GPT-4`, open the PlugIn Options. Open the PlugIn Store and select [Develop your own plugin]
    ![Develop your own plugin in PlugIn Store](/images/02chatgptpluginstore.png "Develop your own plugin in PlugIn Store")

1. Type [http://localhost:5100], and select [Find Manifest File]. ChatGPT will search for the manifest file at [http://localhost:5100/.well-known/ai-plugin.json] and load the plugin information. Select [Install LocalHost PlugIn]
    ![Test plugin manifest information found](/images/03foundpluginforlocalstore.png "Test plugin manifest information found")

1. The PlugIn will now be enabled in ChatGPT
    ![Test plugin installed](/images/04plugininstalled.png "Test plugin installed")

1. We can now test the plug in. ChatGPT will use the Pet plugin for pets related questions
    1. Sample Question, "Who owns the oldest Pet?"
    ![Who owns the oldest pet?](/images/05oldestpets.png "Who owns the oldest pet?")
    
    1. Sample Question, "Who has more than one pet?"
    ![Who has more than one pet?](/images/06morethan1pet.png "Who has more than one pet?")


## Test in ChatGPT

You can use the following prompts to test the plugin in ChatGPT:

- How many cats are in the db?
- List the cat names in the catalog
- What info is needed to add a new pet?
- OK, let's add a new pet. His name is Net, is a calico breed, 17-year-old brown cat. He weighs 3 kilograms. His owner is Bruno Capuano, with the same owner information that you have in the database.


## Run in Codespaces (Coming Soon!)
1. Click here to open in GitHub Codespaces

    [![Open in GitHub Codespaces](https://img.shields.io/static/v1?style=for-the-badge&label=GitHub+Codespaces&message=Open&color=lightgrey&logo=github)](https://codespaces.new/elbruno/OpenAI-Plugin-NET-Sample)

1. This action may take a couple of minutes. Once the Codespaces is initialized, check the Extensions tab and check that all extensions are installed.

    **ToDo ADD IMAGE** 

1. Using the  the ***Explorer*** option, open the file `src/OpenAI.PlugIn.NETSample/Program.cs`.

    **ToDo ADD IMAGE** 

1. Using the  the ***Run and Debug*** option, run the program. Select "C# as the run option".

    **ToDo ADD IMAGE** 

1. Open Codespaces Ports tab, right click 8000, and make it public.

    **ToDo ADD IMAGE** 

1. Copy the Codespaces address for port 8000
1. Open Chat GPT and add the plugin with the Codespaces address
1. Run one of the following queries 
    1. *How many cats are in the catalog?*
    1. *Is there anyone who owns more than one pet?*

