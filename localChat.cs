//Originally by Curse
//Modified by ZSNO

//Execute the names list
exec("./names.cs");

if(isfile("Add-Ons/System_ReturnToBlockland/server.cs"))
{
	if(!$RTB::RTBR_ServerControl_Hook)
		exec("Add-Ons/System_ReturnToBlockland/RTBR_ServerControl_Hook.cs");

	if(!$Pref::Server::LCchatDistance)
		$Pref::Server::LCchatDistance = 50;
	if(!$local_chat)
		$local_chat = 1;

	Rtb_registerPref("Chat Distance","Local Chat","$pref::server::lcChatDistance","int 0 100","Server_localChat","15","0","1","localChat_distance");
	Rtb_registerPref("Enabled","Local Chat","$local_chat","bool","Server_localChat","1","0","1","localChat_on");
}
else
{
	$pref::Server::lcChatDistance = 50;
	$pref::Server::lcyChatDistance = 150;
	$local_chat = 1;
}

function serverCmdL(%client)
{
	if(%client.IsSuperAdmin)
	{
		if($localchat_toggle == 1)
		{
			$local_chat = 1;
			$localchat_toggle = 0;
			messageAll('',"\c6Local chat turned on!");
		}
		else
		{
			$local_chat=0;
			$localchat_toggle = 1;
			messageAll('',"\c6Local chat turned off!");
		}
	}
}

function randomiserone()
{
	$NameA = getrandom(0,259);
}

function randomisertwo()
{
	$NameB = getrandom(0,259);
}
RegisterPersistenceVar("identity",false,"");

package local_chat
{
	function GameConnection::spawnPlayer(%this)
	{
		parent::spawnPlayer(%this);
		if(%this.player.identity $= "")
		{
			randomiserone();
			randomisertwo();
			%this.player.identity = $firstname[$nameA] SPC $firstrname[$NameB];
		}
	}
	function ServerCmdTeamMessageSent(%client,%text)
	{
		if($local_chat==false)
			parent::ServerCmdTeamMessageSent(%client,%text);

		else
		{
			if(!isObject(%client.player))
			{
				messageClient(%client,'',"\c6You cannot talk without a body!");
				return;
			}
			if($Pref::Server::ETardFilter == 1)
			{
				for(%i=0; %i<getWordCount(%text); %i++)
				{
					%word=getWord(%text,%i);
					for(%j=0; %j<getwordCount($Pref::Server::ETardList); %j++)
						if(%word $= getWord($Pref::Server::ETardList,%j))
						{
							messageClient(%client,'',"\c5This is a civilized game. Please use full words.");
							return;
						}
				}
			}
			serverCmdLocalMessageSent(%client,%text);
		}
	}

	function serverCmdMessageSent(%client,%text)
	{
		if(%client.player.sleeping)
			return;
		if(%client.isAdmin && getSubStr(%text,0,1) $= "^")
		{
			messageAll('',"\c2" @ %client.getPlayerName() @ "\c6:" SPC getSubStr(%text, 1, strLen(%text)-1));
			return;
		}
		if($local_Chat == true && !isObject(%client.player))
		{
			messageClient(%client,'',"\c6You cannot talk without a body!");
			return;
		}

		if($local_chat==true && isObject(%client.player))
		{
			for(%i=0; %i<getWordCount(%text); %i++)
			{
				%word=getWord(%text,%i);
				for(%j=0; %j<getwordCount($Pref::Server::ETardList); %j++)
				{
					if(%word $= getWord($Pref::Server::ETardList,%j))
					{
						messageClient(%client,'',"\c5This is a civilized game. Please use full words.");
						return;
					}
				}
			}
			%client.player.playThread(2,talk);
			%client.player.schedule(1000,playthread,2,root);
			if(getWord(%text,0) $= "yell")
				serverCmdLocalYellMessageSent(%client,getWords(%text,1,getWordCount(%text)-1));
			else
				serverCmdLocalMessageSent(%client,%text);
		}
		if($local_chat == false || !isObject(%client.player))
			parent::serverCmdMessageSent(%client,%text);

	}
	function ServerCmdStartTalking()
    {
    	return;
    }
    function ServerCmdStopTalking()
    {
    	return;
    }
};
activatePackage(local_chat);

function serverCmdlocalMessageSent(%client,%text)
{
	if($local_chat == false || %text $= "")
		return;
	%text=stripMlControlChars(%text);

	if(%client.LastLocalMessage $= %text)
	{
		messageClient(%client,'',"\No spamming.");
		return;
	}
	else
		%client.lastLocalMessage=%text;

	for(%i=0; %i<clientGroup.getCount(); %i++)
	{
		%player=clientGroup.getObject(%i);
		
		if(isObject(%client.player) && isObject(%player.player))
		{
			if(vectorDist(%client.player.getTransform(),%player.player.getTransform())<$pref::Server::lcChatDistance)
				messageClient(%player,'',"\c3" @ %client.player.identity @ " \c3says, \c6\"" @ %text @ "\"");
		}
	}
}

function serverCmdlocalYellMessageSent(%client,%text)
{
	if($local_chat == false || %text $= "")
		return;
	%text=stripMlControlChars(%text);

	if(%client.LastLocalMessage $= %text)
	{
		messageClient(%client,'',"\No spamming.");
		return;
	}
	else
		%client.lastLocalMessage=%text;

	for(%i=0; %i<clientGroup.getCount(); %i++)
	{
		%player=clientGroup.getObject(%i);
		
		if(isObject(%client.player) && isObject(%player.player))
		{
			if(vectorDist(%client.player.getTransform(),%player.player.getTransform())<$pref::Server::lcyChatDistance)
				messageClient(%player,'',"\c3" @ %client.player.identity @ " \c3yells, \c6\"" @ %text @ "\"");
		}
	}
}
