//By ZSNO
//When all bricks loaded, apply material via color

//-----------------------------------------------------
//Brick Materials
//-----------------------------------------------------
function changeBrickColor(%col1, %col2)
{
	if(getBrickCount() == 0)
		return;

	%groupCount = MainBrickGroup.getCount();
	for(%i = 0; %i < %groupCount; %i++)
	{
		%group = MainBrickGroup.getObject(%i);
		%count = %group.getCount();
		for(%j = 0; %j < %count; %j++)
		{
			%brick = %group.getObject(%j);
			if(%brick.getColorID() == %col1)
			%brick.setColor(%col2);
		}
	}
}
//configured for Eksi's colorset
$Survive::grassColor = 35;
$Survive::darkGrassColor = 43;
$Survive::rockColor = 2;
$Survive::sandColor = 7;
$Survive::asphalt = 4;
function setColoredBrickMaterial()
{
	if(getBrickCount() == 0)
		return;

	%groupCount = MainBrickGroup.getCount();
	for(%i = 0; %i < %groupCount; %i++)
	{
		%group = MainBrickGroup.getObject(%i);
		%count = %group.getCount();
		for(%j = 0; %j < %count; %j++)
		{
			%brick = %group.getObject(%j);
			if(%brick.getDatablock().printAspectRatio $= "ModTer")
			{
				switch(%brick.getColorID())
				{
					case $Survive::grassColor: %brick.material = "grass";
					case $Survive::darkGrassColor: %brick.material = "grass";
					case $Survive::rockColor: %brick.material = "gravel";
					case $Survive::sandColor: %brick.material = "sand";
					case $Survive::asphalt: %brick.material = "dirt";
				}
			}
		}
	}
}

package WhatRemainsMats
{
	function ServerLoadSaveFile_End()
	{
		parent::ServerLoadSaveFile_End();
		setColoredBrickMaterial();
	}
};
activatePackage(WhatRemainsMats);