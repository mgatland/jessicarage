	private var wasLocked = false;
	function Update () {

		if (Input.GetKeyDown ("escape"))
			Screen.lockCursor = false;
		
		if (Input.GetMouseButtonDown(0))
			Screen.lockCursor = true;
	}