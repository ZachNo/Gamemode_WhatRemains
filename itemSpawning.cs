//By ZSNO

//Survive Item spawning
//Put item spawning config files if /config/surviveItems/
//format for items is [ItemDBName]SPC[probability]
//Probability of all items must not exceed 100
//Anything less than 100 with be filled automatically with nothing
//Current valid files are:
//medical
//military
//gas
//general
//food

$Survive::ItemSpawnTimeout = 420000;
$Survive::ItemSpawnRadius = 200;

function importSurviveItems()
{
	echo("Importing What Remains item spawn lists...");
	%pattern = "config/surviveItems/*.txt";
	%file = findFirstFile(%pattern);
	while(%file !$= "")
	{
		%f = new FileObject();
		%f.openForRead(%file);
		%i = 0;
		%accuProb = 0;
		%filename = strReplace(%file,"config/surviveItems/","");
		%filename = strReplace(%filename,".txt","");
		while(!%f.iseof())
		{
			%line = %f.readLine();
			%varname = %filename @ "_" @ %i @ "item";
			$Survive::Item[%varname] = getWord(%line,0);
			%varname = %filename @ "_" @ %i @ "prob";
			%prob = getWord(%line,1);
			$survive::Item[%varname] = %prob + %accuProb;
			%accuProb += %prob;
			%i++;
		}
		$Survive::Item[%filename] = %i;
		echo( %i @ " items imported from " @ %file @ ".txt");
		%file = findNextFile(%pattern);
	}
}

function fxDTSBrick::spawnSurviveItem(%this)
{
	%item = getRandomSurviveItem(%this.spawnType);
	if(!isObject(%item))
		return;
	if(%this.spawnType !$= "vehicle")
	{
		%ranX = ((getRandom(0,8) - 4)/2) + getWord(%this.position,0);
		%ranY = ((getRandom(0,8) - 4)/2) + getWord(%this.position,1);
		%this.spawnedItem = new Item()
		{
			datablock = %item;
			position = %ranX SPC %ranY SPC getWord(%this.position,2)+1;
		};
	}
	else if(!isObject(%this.vehicle))
	{
		%this.setVehicle(%item.getID());
		%this.spawnVehicle();
	}
}

function fxDTSBrick::checkToSpawn(%this)
{
	initContainerRadiusSearch(%this.getPosition(),$Survive::ItemSpawnRadius,$TypeMasks::PlayerObjectType);
	if(containerSearchNext())
	{
		if(%this.playerInRange)
		{
			%this.schedule($Survive::ItemSpawnTimeout,checkToSpawn);
			return;
		}
		%this.playerInRange = 1;
		
		%this.spawnSurviveItem();
		%this.schedule($Survive::ItemSpawnTimeout,checkToSpawn);
		return;
	}
	else
	{
		%this.playerInRange = 0;
		if(isObject(%this.spawnedItem))
			%this.spawnedItem.delete();
		%this.spawnedItem = 0;
	}
	%this.schedule(10000 + getRandom(0,100),checkToSpawn);
}

function getRandomSurviveItem(%list)
{
	%rand = getRandom(0,1000)/10;
	for(%i=0;%i<$Survive::Item[%list];%i++)
	{
		%varname = %list @ "_" @ %i @ "prob";
		if($Survive::Item[%varname] > %rand)
		{

			%varname = %list @ "_" @ %i @ "item";
			return $Survive::Item[%varname];
		}
	}
	return 0;
}

package SurviveItemSpawning
{
	function fxDTSBrick::onPlant(%this)
	{
		parent::onPlant(%this);
		if(%this.getDatablock() == SurviveMedicalBrick.getID())
		{
			%this.spawnType = "medical";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveMilitaryBrick.getID())
		{
			%this.spawnType = "military";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveGasSBrick.getID())
		{
			%this.spawnType = "gas";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveGeneralBrick.getID())
		{
			%this.spawnType = "general";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveFoodBrick.getID())
		{
			%this.spawnType = "food";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveVehicleBrick.getID())
		{
			%this.spawnType = "vehicle";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
	}
	function fxDTSBrick::onLoadPlant(%this)
	{
		parent::onLoadPlant(%this);
		if(%this.getDatablock() == SurviveMedicalBrick.getID())
		{
			%this.spawnType = "medical";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveMilitaryBrick.getID())
		{
			%this.spawnType = "military";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveGasSBrick.getID())
		{
			%this.spawnType = "gas";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveGeneralBrick.getID())
		{
			%this.spawnType = "general";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveFoodBrick.getID())
		{
			%this.spawnType = "food";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
		if(%this.getDatablock() == SurviveVehicleBrick.getID())
		{
			%this.spawnType = "vehicle";
			%this.checkToSpawn();
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
		}
	}
	function fxDTSBrick::spawnVehicle(%this,%delay)
	{
		if(%delay)
			return;
		parent::spawnVehicle(%this,%delay);
	}
};
activatePackage(SurviveItemSpawning);

datablock fxDTSBrickData(SurviveMilitaryBrick)
{
	brickFile = "base/data/bricks/flats/8x8F.blb";
	category = "Special";
	subCategory = "What Remains";
	uiName = "Military Loot Spawn ";
};
datablock fxDTSBrickData(SurviveMedicalBrick)
{
	brickFile = "base/data/bricks/flats/8x8F.blb";
	category = "Special";
	subCategory = "What Remains";
	uiName = "Medical Loot Spawn ";
};
datablock fxDTSBrickData(SurviveGasSBrick)
{
	brickFile = "base/data/bricks/flats/8x8F.blb";
	category = "Special";
	subCategory = "What Remains";
	uiName = "Gas Station Loot Spawn ";
};
datablock fxDTSBrickData(SurviveGeneralBrick)
{
	brickFile = "base/data/bricks/flats/8x8F.blb";
	category = "Special";
	subCategory = "What Remains";
	uiName = "General Loot Spawn ";
};
datablock fxDTSBrickData(SurviveFoodBrick)
{
	brickFile = "base/data/bricks/flats/8x8F.blb";
	category = "Special";
	subCategory = "What Remains";
	uiName = "Food Loot Spawn ";
};
datablock fxDTSBrickData(SurviveVehicleBrick)
{
	brickFile = "base/data/bricks/special/vehicleSpawn.blb";
	category = "Special";
	subCategory = "What Remains";
	uiName = "Vehicle Spawn ";
	specialBrickType = "vehicleSpawn";
};

importSurviveItems();