	datablock ItemData(fpRedsodaItem)
	{
		category = "Weapon";  // Mission editor category
		className = "Weapon"; // For inventory system

		 // Basic Item Properties
		shapeFile = "./fpRedsoda.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;

		//gui stuff
		uiName = "Red Soda";
		iconName = "./Icon_fpRedsoda";
		doColorShift = false;

		 // Dynamic properties defined by the scripts
		image = fpRedsodaImage;
		canDrop = true;
	};

	datablock ShapeBaseImageData(fpRedsodaImage)
	{
	   // Basic Item properties
	   shapeFile = "./fpRedsoda.dts";
	   emap = true;

	   // Specify mount point & offset for 3rd person, and eye offset
	   // for first person rendering.
	   mountPoint = 0;
	   offset = "-.05 0 0";
	   eyeOffset = 0; //"0.7 1.2 -0.5";
	   rotation = eulerToMatrix( "-90 90 0" );

	   className = "WeaponImage";
	   item = fpRedsodaItem;

	   //raise your arm up or not
	   armReady = true;

	   doColorShift = false;

	   // Initial start up state
		stateName[0]                     = "Ready";
		stateTransitionOnTriggerDown[0]  = "Fire";
		stateAllowImageChange[0]         = true;

		stateName[1]                     = "Fire";
		stateTransitionOnTimeout[1]      = "Ready";
		stateAllowImageChange[1]         = true;
		  stateScript[1]                   = "onFire";
		stateTimeoutValue[1]		   = 1;
		stateSound[1]					= drinkSound;
	};

	function fpRedsodaImage::onFire(%this,%obj,%slot)
	{
		

		for(%i=0;%i<5;%i++)
		{
			%toolDB = %obj.tool[%i];
			if(%toolDB $= %this.item.getID())
			{
				%obj.addThirst(15);
				%obj.tool[%i] = 0;
				%obj.weaponCount--;
				messageClient(%obj.client,'MsgItemPickup','',%i,0);
				serverCmdUnUseTool(%obj.client);
				break;
			}
		}
	}