//By Dephurites

//=======================================================
//Vehicle Gas Mod V1.0
//To set a custom amount of gas for a vehicle you are making, use this:
//VGM_Gas = CustomAmount;
//=======================================================
//Version:
//Major = 	1
//Minor =	3
//Patch =	0
//Total =	1.3.0
//=======================================================

registerOutputEvent("fxDTSBrick", "addGas", "int 1 1000 1", 0);
registerOutputEvent("fxDTSBrick", "setGas", "int 1 1000 1", 0);
registerOutputEvent("Vehicle", "addGas", "int 1 1000 1", 1);
registerOutputEvent("Vehicle", "setGas", "int 1 1000 1", 1);
$VGM::DefaultGas = 200;

if(isFile("Add-Ons/Event_Variables/changelog.txt") && $AddOn__Event_Variables)
{
	if(!isfunction("registerSpecialVar"))
	{
		exec("Add-Ons/Event_Variables/server.cs");
		$AddOnLoaded__Event_Variables = 1;
	}
	registerSpecialVar(Vehicle, "gas", "%this.VGM_Gas");
}

function VGM_tick()
{
	if(!$VGM::Enabled)
		return;
	
	for(%a = 0; $VGM::Vechs >= %a; %a++)
	{
		%vech = $VGM::Vech[%a];
		if(!isobject(%vech))
		{
			$VGM::Vech[%a] = "";
			continue;
		}
		%oldx 	= getword(%vech.VGM_OldPos, 0)/8;
		%oldy 	= getword(%vech.VGM_OldPos, 1)/8;
		%oldz 	= getword(%vech.VGM_OldPos, 2)/8;
		%x		= getword(%vech.position, 0)/8;
		%y		= getword(%vech.position, 1)/8;
		%z		= getword(%vech.position, 2)/8;
		
		%xchange = mAbs(%x - %oldx);
		%ychange = mAbs(%y - %oldy);
		%zchange = %z - %oldz;
		if(%zchange > 0)
			%zchange = %zchange*3;
		else
			%zchange = 0;
		
		%subtractGas = %xchange + %ychange + %zchange;
		%vech.VGM_Gas -= %subtractGas;
		%vech.VGM_LastSub = %subtractGas;
		for(%b = 0; %b < %vech.getmountedobjectcount(); %b++)
		{
			%cl = %vech.getmountedobject(%b).client;
			if(%vech.VGM_Gas <= 0)
			{
				%vehicle.VGM_Gas = 0;
				centerprint(%cl, "Your vehicle is out of gas!", 5);
				%cl.player.setcontrolobject(%cl.player);
			}
			else
			{
				centerprint(%cl, "Fuel: \c3" @ mCeil(%vech.VGM_Gas), 1);
			}
		}
		%vech.VGM_OldPos = %vech.position;
	}
	$VGM::Sch = schedule(500, 0, VGM_Tick);
}

package VGM
{
	function fxDTSbrick::spawnVehicle(%this)
	{
		parent::spawnVehicle(%this);
		%vehicle = %this.vehicle;
		if(!isObject(%vehicle))
			return;
		if(%vehicle.getclassname() $= "AIPlayer")
			return;
		
		%loop = 0;
		while(isobject($VGM::Vech[%loop]))
		{
			%loop++;
		}
		if(%loop > $VGM::HighestNumb)
		{
			$VGM::HighestNumb = %loop;
			$VGM::Vechs = %loop;
		}
		$VGM::Vech[%loop] = %vehicle;
		%vehicle.VGM_OldPos = %vehicle.position;
		if(%vehicle.getdatablock().VGM_Gas)
			%vehicle.VGM_Gas = %vehicle.getdatablock().VGM_Gas;
		else
			%vehicle.VGM_Gas = $VGM::DefaultGas;
		
		%vehicle.VGM = 1;
	}
	
	function vehicle::mountobject(%this, %obj, %slot)
	{
		parent::mountobject(%this, %obj, %slot);
		if(%this.VGM_Gas <= 0 && %this.VGM)
		{
			//Your not in control, no gas!
			%obj.setcontrolobject(%obj);
			centerprint(%obj.client, "Your vehicle is out of gas!", 5);
		}
	}
	
	function destroyServer()
	{
		cancel($VGM::Sch);
		for(%a = 0; $VGM::Vechs >= %a; %a++)
		{
			$VGM::Vech[%a] = "";
		}
		$VGM::Vechs = 0;
		parent::destroyServer();
	}
};
activatepackage(VGM);

function fxDTSBrick::AddGas(%this, %gas)
{
	if(isobject(%vehicle = %this.vehicle))
		%vehicle.VGM_Gas += %gas;
}

function fxDTSBrick::SetGas(%this, %gas)
{
	if(isobject(%vehicle = %this.vehicle))
		%vehicle.VGM_Gas = %gas;
}

function Vehicle::AddGas(%vehicle, %gas)
{
	%vehicle.VGM_Gas += %gas;
}

function Vehicle::SetGas(%vehicle, %gas)
{
	%vehicle.VGM_Gas = %gas;
}

cancel($VGM::Sch);
VGM_tick();