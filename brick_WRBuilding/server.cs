//By ZSNO
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
	uiName = "WRVehicle Spawn ";
	specialBrickType = "vehicleSpawn";
};
datablock fxDTSBrickData (BrickZombieWR_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Holes";
	uiName = "WRZombie Hole";
	iconName = "Add-Ons/Bot_Zombie/icon_zombie";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	holeBot = "WRZombieHoleBot";
};
