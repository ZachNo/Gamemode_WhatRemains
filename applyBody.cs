function applyBodyPart(%client)
{
	if(!isObject(%client.player))
		return;
	
	%player = %client.player;
	
	if(!(%player.getDatablock().shapeFile $= "m.dts"))
		return;
		
	%player.hideAllNodes();
	
	%accentName = $Accent[%client.accent];
	if(!(%accentName $= "None"))
		%player.unHideNode(%accentName);
	
	%hatName = $Hat[%client.hat];
	if(!(%hatName $= "None"))
		%player.unHideNode(%hatName);
		
	%packName = $Pack[%client.pack];
	if(!(%packName $= "None"))
		%player.unHideNode(%packName);
		
	%chestName = $Chest[%client.chest];
	%player.unHideNode(%chestName);
	
	%secondpackName = $SecondPack[%client.secondpack];
	if(!(%secondpackName $= "None"))
		%player.unHideNode(%secondpackName);
		
	%hipName = $Hip[%client.hip];
	%player.unHideNode(%hipName);
	if(%hipName $= "SkirtHip")
	{
		%player.unHideNode("skirtTrimLeft");
		%player.unHideNode("skirtTrimRight");
	}
		
	%llegName = $LLeg[%client.lleg];
	if(!(%llegName $= "None"))
		%player.unHideNode(%llegName);
		
	%rlegName = $RLeg[%client.rleg];
	if(!(%rlegName $= "None"))
		%player.unHideNode(%rlegName);
		
	%larmName = $LArm[%client.larm];
	%player.unHideNode(%larmName);
		
	%rarmName = $RArm[%client.rarm];
	%player.unHideNode(%rarmName);
	
	%lhandName = $LHand[%client.lhand];
	%player.unHideNode(%lhandName);
		
	%rhandName = $RHand[%client.rhand];
	%player.unHideNode(%rhandName);
}

function applyBodyColors(%client)
{
	if(!isObject(%client.player))
		return;
	
	%player = %client.player;
	
	if(!(%player.getDatablock().shapeFile $= "m.dts"))
		return;
	
	%accentName = $Accent[%client.accent];
	if(!(%accentName $= "None"))
		%player.setNodeColor(%accentName, %client.accentColor);
		
	%hatName = $Hat[%client.hat];
	if(!(%hatName $= "None"))
		%player.setNodeColor(%hatName, %client.hatColor);
		
	%player.setNodeColor("headSkin", %client.headColor);
	
	%llegName = $LLeg[%client.lleg];
	if(!(%llegName $= "None"))
		%player.setNodeColor(%llegName, %client.llegColor);
		
	%rlegName = $RLeg[%client.rleg];
	if(!(%rlegName $= "None"))
		%player.setNodeColor(%rlegName, %client.rlegColor);
		
	%chestName = $Chest[%client.chest];
	%player.setNodeColor(%chestName, %client.chestColor);
	
	%hipName = $Hip[%client.hip];
	%player.setNodeColor(%hipName, %client.hipColor);
	if(%hipName $= "SkirtHip")
	{
		%player.setNodeColor("skirtTrimLeft", %client.llegColor);
		%player.setNodeColor("skirtTrimRight", %client.rlegColor);
	}
	
	%larmName = $LArm[%client.larm];
	%player.setNodeColor(%larmName, %client.larmColor);
	
	%rarmName = $RArm[%client.rarm];
	%player.setNodeColor(%rarmName, %client.rarmColor);
	
	%lhandName = $LHand[%client.lhand];
	%player.setNodeColor(%lhandName, %client.lhandColor);
		
	%rhandName = $RHand[%client.rhand];
	%player.setNodeColor(%rhandName, %client.rhandColor);
		
	%packName = $Pack[%client.pack];
	if(!(%packName $= "None"))
		%player.setNodeColor(%packName, %client.packColor);
		
	%secondpackName = $SecondPack[%client.secondpack];
	if(!(%secondpackName $= "None"))
		%player.setNodeColor(%secondpackName, %client.secondpackColor);
		
	%player.setFaceName(%client.faceName);
	%player.setDecalName(%client.decalName);
}