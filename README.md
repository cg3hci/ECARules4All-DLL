This folder contains the core files for instantiating the Rule Engine inside any template, plus a few examples of Objects

* [Core Elements](#core-elements)
* [How to Install](#how-to-install)
* [How to send Events](#how-to-send-events)

# Core Elements
These files have to be imported all together to get the engine up and running.
* **RuleEngine**: This file contains all the code that implements the Rule Engine itself, the Event Bus and the *Rule*, *Action* and *Condition*. The file is also contains documentation comments, in order to suggest code completion where supported;
* **RuleEngineLoader**: This file instantiates at runtime the Rule Engine and the Event Bus, in order to be sure that they are properly running from the start;
* **\[ ECARules4All | Action | StateVariable \]Attribute**: These files are the custom attributes needed to decorate the Objects so the Rule Engine can address their variables and functions;
* **ECAObject**: The root Object from which all the other Objects can derive;
* **Objects**: Contains definitions for *Position, Rotation, Path, Time* and *Color* classes;
* **ECARules4AllType**: This file will contain extra informations intended for the Rule Editor UI, as of now it's here for predisposition.

# How to Build and Install

Building and installing the Rule Engine in a Unity application is straightforward and consists of two parts: project side and Unity side.

## Project Side
- Clone this repository.
- Build the project.
- The generated DLLs will be available in the `./bin` folder.

## Unity Side
- Inside the `Assets` folder, create a subfolder of your choice, preferably named `dlls`.
- Import all the DLLs generated in the previous step into that folder, except for `UnityEngine.dll`.


# How to Connect to an XR Rule Engine

To connect your Unity application and the VR Rule Engine to the [XR Rule Engine](https://github.com/AlessandroCarcangiu/eud4xr), use the `HomeAssistantClient` and `ApiServer` classes.

- `ApiServer`: allows you to start the REST server used to receive requests and information from the XR Rule Engine through Home Assistant. You will need to specify the port on which the server should run and listen for incoming connections.
  `APIServer _apiServer = new APIServer(8080);`
- `HomeAssistantClientV2`: creates the singleton instance of `HomeAssistantClient`. Initializing this instance is essential to allow the VR Rule Engine to send data to the XR Rule Engine. To do so, you need the Home Assistant platform URL and a security token (see the [official authentication documentation](https://www.home-assistant.io/docs/authentication/)). Once the client has been initialized, you can subscribe to the events that the XR Rule Engine is configured to send to the VR Rule Engine's server.
  > `AbstractClient<HomeAssistantClient> hassClient = AbstractClient<HomeAssistantClient>.GetInstance();`
  > `_apiServer.ActionUpdate += ((HomeAssistantClient)hassClient).ReceivedUpdateHandler;`

# How to send Events
Whenever you need to fire an event to the Rule Engine you can do it by using
`EventBus.GetInstance().Publish(new Action(/*Action Syntax here*/))`

> Be sure to include (or have included by Unity) the following line of code at the start of your script:
>
> `using ECARules4All.RuleEngine;`

The **Action** declaration is structured like the following:

| Name | Mandatory? | Description |
|---|---|---|
|**Subject**|Yes|Always a GameObject, it tells who made the action in the scene|
|**Verb** |Yes| A string that describes the action made by the subject|
|**Object**|No| Additional values (e.g.: a GameObject, a string, a primitive type, etc.) used as parameters inside a function, or to define a variable to modify (with the *Quantity* field)|
|**Quantity**|No[^1]|This field defines the value used to alter (by assignment or decrement/increment) the value indicated in the *Object* field. |

# How to add Rules
If you need to add Rules the syntax for adding one is the following:

`RuleEngine.GetInstance().Add(new Rule(/*Rule Syntax Here*/))`

The Rule declaration is structured like the following:

| Name | Mandatory? | Type | Description |
|---|---|---|---|
|**When**|Yes|`Action`|An Action that defines what needs to happen for this Rule to trigger|
|**If**|No|`Condition`|A Condition to be satisfied in order to execute the events in the *Then* list|
|**Then**|Yes|`List<Action>`|The list of actions that this rule will trigger|

[^1]: It is if modifying a value
