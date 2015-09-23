//By ZSNO
//Vehicles play sounds depending on if someone is in them and they're moving

datablock audioDescription(audioFarLooping3d : audioCloseLooping3D)
{
	volume = 0.75;
	referenceDistance = 100;
	maxDistance = 300;
	coneVector = "0 0 0";
};
datablock audioDescription(audioFar3d : audioFarLooping3D)
{
	isLooping = 0;
};
datablock audioProfile(engineIdleSound)
{
	filename = "./sounds/vehicle_idle.wav";
	description = audioFarLooping3D;
	preload = 1;
};
datablock audioProfile(engineStartSound : engineIdleSound)
{
	filename = "./sounds/vehicle_startup.wav";
	description = audioFar3D;
};
datablock audioProfile(engineStopSound : engineStartSound)
{
	filename = "./sounds/vehicle_stop.wav";
};

function WheeledVehicle::engineSoundCheck(%this)
{
	for(%i=0;%i<%this.getMountedObjectCount();%i++)
	{
		if(%this.getMountedObjectNode(%i) == 0)
			%this.playAudio(0,engineIdleSound);
	}
}

package vehicleEngine
{
	function armor::onMount(%this,%obj,%mount,%slot)
	{
		parent::onMount(%this,%obj,%mount,%slot);
		if(%mount.getClassname() $= "WheeledVehicle" && !(%mount.getDatablock().shapeFile $= "Add-Ons/Item_Skis/deathVehicle.dts"))
		{
			if(%slot == 0)
			{
				%mount.playAudio(0,engineStartSound);
				%mount.schedule(4000,engineSoundCheck);
			}
		}
	}
	function WheeledVehicleData::onDriverLeave(%this,%obj,%driver)
	{
		parent::onDriverLeave(%this,%obj,%driver);
		%obj.playAudio(0,engineStopSound);
	}
		
};
activatePackage(vehicleEngine);