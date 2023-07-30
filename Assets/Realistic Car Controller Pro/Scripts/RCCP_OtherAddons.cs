//----------------------------------------------
//        Realistic Car Controller Pro
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Other addons belongs to the vehicle, such as nos, dashboard, interior, cameras, exhaust, AI, recorder, attacher, etc...
/// </summary>
public class RCCP_OtherAddons : MonoBehaviour {

    private RCCP_CarController CarController {

        get {

            if (!componentsTaken && _carController == null)
                _carController = GetComponentInParent<RCCP_CarController>(true);

            return _carController;

        }

    }

    public RCCP_Nos Nos {

        get {

            if (!componentsTaken && _nos == null)
                _nos = GetComponentInChildren<RCCP_Nos>(true);

            return _nos;

        }
        set {

            _nos = value;

        }

    }

    public RCCP_Visual_Dashboard Dashboard {

        get {

            if (!componentsTaken && _dashboard == null)
                _dashboard = GetComponentInChildren<RCCP_Visual_Dashboard>(true);

            return _dashboard;

        }
        set {

            _dashboard = value;

        }

    }

    public RCCP_Exterior_Cameras ExteriorCameras {

        get {

            if (!componentsTaken && _exteriorCameras == null)
                _exteriorCameras = GetComponentInChildren<RCCP_Exterior_Cameras>(true);

            return _exteriorCameras;

        }
        set {

            _exteriorCameras = value;

        }

    }

    public RCCP_Exhausts Exhausts {

        get {

            if (!componentsTaken && _exhausts == null)
                _exhausts = CarController.GetComponentInChildren<RCCP_Exhausts>(true);

            return _exhausts;

        }
        set {

            _exhausts = value;

        }

    }

    public RCCP_AI AI {

        get {

            if (!componentsTaken && _AI == null)
                _AI = GetComponentInChildren<RCCP_AI>(true);

            return _AI;

        }
        set {

            _AI = value;

        }

    }

    public RCCP_Recorder Recorder {

        get {

            if (!componentsTaken && _recorder == null)
                _recorder = GetComponentInChildren<RCCP_Recorder>(true);

            return _recorder;

        }
        set {

            _recorder = value;

        }

    }

    public RCCP_TrailerAttacher TrailAttacher {

        get {

            if (!componentsTaken && _trailerAttacher == null)
                _trailerAttacher = GetComponentInChildren<RCCP_TrailerAttacher>(true);

            return _trailerAttacher;

        }
        set {

            _trailerAttacher = value;

        }

    }

    private RCCP_CarController _carController;
    private RCCP_Nos _nos;
    private RCCP_Visual_Dashboard _dashboard;
    private RCCP_Exterior_Cameras _exteriorCameras;
    private RCCP_Exhausts _exhausts;
    private RCCP_AI _AI;
    private RCCP_Recorder _recorder;
    private RCCP_TrailerAttacher _trailerAttacher;

    public bool componentsTaken = false;

    private void OnEnable() {

        StartCoroutine(ComponentsTaken());

        if (CarController)
            CarController.OtherAddonsManager = this;
        else
            enabled = false;

    }

    private IEnumerator ComponentsTaken() {

        componentsTaken = false;
        yield return new WaitForSeconds(.04f);
        componentsTaken = true;

    }

    public void UpdateComponents() {

        StartCoroutine(ComponentsTaken());

    }

}
