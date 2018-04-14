# SitecoreDeploy

This utility expects **Sitecore v9** or greater and **MicroCHAP v1.2.2** or greater.

A utility to execute commands during deployment, such as Publish, Rebuild indexes and so on.

This utility is heavily inspired by Unicorn.  I have used microCHAP and ripped the authentication code out of Unicorn.

It is intended that you use this utility via PowerShell commands during your deployment pipeline.

## Using SitecoreDeploy commands

After you have installed and built the project you can then call the commands via PowerShell, curl, PostMan or other tool.

You can test the installation of SitecoreDeploy by visiting /sitecoredeploy.aspx?command=challenge 


## Available commands

When a request to execute a command is made and query string parameters are turned into a Dictionary<string,string> and passed into the commands Execute method.
This allows you pass well known paramters from your PowerShell script to the command.

For example, the Publish command understands ?arg=Smart|Full

Whenever you include a query string argument you **must** also include it when creating a challenge and signature with microCHAP.  The PowerShell example above does this.


#### Publish 

Publish the site.

By default this will perform a smart publish.  Each language is published.

|Parameter|Value|
|:-------|:-------|
|command|publish|
|arg|smart, full or incremental|

e.g. /sitecoredeploy.aspx?command=Publish&arg=full

#### RebuildIndexes 

Rebuild the sitecore search indexes.

By default this rebuild all indexes.

|Parameter|Value|
|:-------|:-------|
|command|rebuildindexes|
|arg|the index id, e.g. sitecore_web_index|

e.g. /sitecoredeploy.aspx?command=RebuildIndexes&arg=web

#### RebuildLinkDatabases 

Rebuild the link databases.  As of Sitecore v9 this is a required post installation step.

By default this will build core, master and web databases.  To build a specific database use the queryString parameter

|Parameter|Value|
|:-------|:-------|
|command|rebuildlinkdatabases|
|arg|the database name, e.g. web, master, core|

e.g. /sitecoredeploy.aspx?command=RebuildLinkDatabases&arg=master

#### DeployMarketingDefinitions 

TBD

|Parameter|Value|
|:-------|:-------|
|command|deploymarketingdefinitions|

e.g. /sitecoredeploy.aspx?command=deploymarketingdefinitions


#### Calling from PowerShell

<pre><code>
    $ErrorActionPreference = 'Stop'

    # you need to set the path to the MicroCHAP.dll
    Add-Type -Path "C:\inetpub\wwwroot\pv.sc\bin\MicroCHAP.dll"

    Function SitecoreDeploy-Execute {
	    Param(
		    [Parameter(Mandatory=$True)]
		    [string]$SitecoreDeployUrl,

		    [Parameter(Mandatory=$True)]
		    [string]$SharedSecret,

		    [Parameter(Mandatory=$True)]
		    [string]$Command,

            [Parameter(Mandatory=$False)]
		    [string]$CommandArguments
	    )

	    # PARSE THE URL TO REQUEST
	    $parsedConfigurations = ($Configurations) -join "^"

	    $commandUrl = "{0}?command={1}&{2}" -f $SitecoreDeployUrl, $Command, $CommandArguments

	    Write-Host "SitecoreDeploy-Execute: Preparing authorization for $commandUrl"

	    # GET AN AUTH CHALLENGE
	    $challenge = Get-Challenge -SitecoreDeployUrl $SitecoreDeployUrl

	    Write-Host "SitecoreDeploy-Execute: Received challenge: $challenge"

	    # CREATE A SIGNATURE WITH THE SHARED SECRET AND CHALLENGE
	    $signatureService = New-Object MicroCHAP.SignatureService -ArgumentList $SharedSecret

	    $signature = $signatureService.CreateSignature($challenge, $commandUrl, $null).SignatureHash

	    Write-Host "SitecoreDeploy-Execute: Created signature $signature, executing $Command..."

	    # USING THE SIGNATURE, EXECUTE THE COMMAND
	    $result = Invoke-RestMethod -Uri $commandUrl -Headers @{ "X-MC-MAC" = $signature; "X-MC-Nonce" = $challenge } -TimeoutSec 10800 -UseBasicParsing

	    $result
    }

    Function Get-Challenge {
	    Param(
		    [Parameter(Mandatory=$True)]
		    [string]$SitecoreDeployUrl
	    )

	    $url = "$($SitecoreDeployUrl)?command=Challenge"

	    $result = Invoke-RestMethod -Uri $url -TimeoutSec 360 -UseBasicParsing

	    $result
    }

    # Execute the publish command with default arguments
    SitecoreDeploy-Execute -SitecoreDeployUrl "https://parks.local/sitecoredeploy.aspx" -SharedSecret "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" -Command "publish"

</code></pre>


## Adding new commands

To add a new command

1. Implement a command, deriving from **SitecoreDeploy.Commands.SitecoreDeployCommand**

e.g.

<pre><code>
    public class DeployMarketingDefinitions : SitecoreDeployCommand
    {
        public DeployMarketingDefinitions() : base("DeployMarketingDefinitions") { }

        public override SitecoreDeployCommandArguments Execute(SitecoreDeployCommandArguments args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            args.Result = "SitecoreDeploy: DeployMarketingDefinitions is not implemented yet!!";
            
            return args;
        }
    }
</code></pre>

2. Register the command under the /sitecore/siteCoreDeploy/commandList section in the sitecore configuration.

<pre><code>
&lt;?xml version="1.0"?&gt;
&lt;configuration xmlns:patch="http://www.sitecore.net/xmlconfig/"&gt;
&lt;sitecore&gt;
    &lt;sitecoreDeploy&gt;
      &lt;commandList&gt;
        &lt;commands&gt;
          &lt;command type="SitecoreDeploy.Commands.DeployMarketingDefinitions, SitecoreDeploy" /&gt;
	&lt;/commands&gt;
       &lt;/commandList&gt;
     &lt;/sitecoreDeploy&gt;
&lt;sitecore&gt;
</code></pre>

## Configuration

Configuration is contained in the app_config/Include/SitecoreDeploy/SitecoreDeploy.config file.

The SitecoreDeploy config registers a pipeline processor in the httpRequestBegin section.  This responds to any requests for the /sitecoredeploy.aspx route.

It is important to set the shared secret in the config and any corresponding PowerShell scripts.