//By ZSNO

//-----------------------------------------------------
//Floating Bricks support
//-----------------------------------------------------
$floatingBricks = 0;
package floatingBricks
{
	function serverCmdplantBrick(%client)
	{
		if($floatingBricks)
		{
			%brick = %client.player.tempBrick;
			if(!isObject(%brick))
				return;
			echo(%brick.checkPlantingError());
			if(%brick.checkPlantingError() $= "float")
			{
				
				%b = new fxDTSBrick()
				{
					position = %brick.position;
					datablock = %brick.dataBlock;
					colorID = %brick.colorID;
					isPlanted = 1;
					client = %client;
					rotation = %brick.rotation;
					scale = "1 1 1";
					angleID = %brick.angleID;
					colorfxID = %brick.colorfxID;
					shapefxID = %brick.shapefxID;
					stackBL_ID = %client.getBLID();
					isBaseplate = %brick.isBaseplate;
				};
				%b.plant();
				%b.setTrusted(1);
				%client.brickGroup.add(%b);
			}
			else
			{
				parent::serverCmdPlantBrick(%client);
			}
		}
		else
		{
			parent::serverCmdPlantBrick(%client);
		}
	}
};
activatePackage(floatingBricks);

function fxDTSBrick::checkPlantingError(%temp)
{
	if(%temp.isPlanted())
	{
		error("You must use a temp brick. Not a brick!");

		return -1;
	}

	%brick = new fxDTSBrick()
	{
		datablock = %temp.getDatablock();
		position = %temp.getPosition();
		rotation = getWords(%temp.getTransform(), 3, 6);
	};

	%error = %brick.plant();
	%brick.delete();
	switch$(%error)
	{
		case 0: return 0;
		case 1: return "overlap";
		case 2: return "float";
		case 3: return "stuck";
		case 4: return "unstable";
		case 5: return "buried";
		default: return "forbidden";
	}
}

function servercmdfloatingbricks(%client,%i)
{
	if(%client.isSuperAdmin)
	{
		if(%i)
		{
			$floatingBricks = 1;
			messageAll('',"Floating bricks enabled!");
		}
		else
		{
			$floatingBricks = 0;
			messageAll('',"Floating bricks disabled!");
		}
	}
}

datablock ShapeBaseImageData(removeWandImage : wandImage)
{
	colorShiftColor = "1 0 1 1";
};

function removeWandImage::onFire(%this,%obj,%slot)
{
	AdminWandImage::onFire(%this,%obj,%slot);
}

datablock ItemData(removeWandItem : wandItem)
{
	uiName = "Removal Wand ";
	image = removeWandImage;
	colorShiftColor = "1 0 1 1";
};

function removeWandImage::onMount(%t,%o)
{
	fixArmReady(%o);
}
 
function removeWandImage::onPreFire(%t,%o)
{
	WandImage.onPreFire(%o);
}
 
function removeWandImage::onStopFire(%t,%o)
{
	%o.playThread(2,root);
}

function removeWandImage::onHitObject(%this,%obj,%slot,%col,%pos,%normal,%shotVec,%crit)
{
	if(%col.getClassName() $= "fxDTSBrick")
	{
		if(getTrustLevel(%obj,%col) < 2)
		{
			%obj.client.centerPrint("You need full trust for that.",2);
			return;
		}
		%col.delete();
	}
}

function serverCmdremoveWand(%client)
{
	if(!isObject(%client.player) || $floatingbricks == 0 || !%client.isAdmin)
		return;
	%client.player.mountImage(removeWandImage,0);
}