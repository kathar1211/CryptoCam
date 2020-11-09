using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public struct Photograph
{
    //store information about the photo
    public int subjectCount; //number of cryptids in the photo
    public bool facinForward; //whether or not the main subject is facing the camera
    public bool coolPose; //whether or not the main subject is doing a special animation
    public string subjectName; //type of cryptid
    public int baseScore; //base score for type of cryptid
    public float distanceFromCenter; //distance of subject from center of frame ((.5,.5) in viewport coordinates)
    public float distanceFromCamera; //distance of subject from camera (viewport.z value)
    public float visibility; //represents how many points on cryptid are visible
    public Texture2D pic; //the actual photo
    public int finalScore;
}

public class Photography : MonoBehaviour {

    public int maxPics;
    Photograph[] allPics; //store pictures as they're taken    
  
    int picIndex; //where we are in our photo taking
    public Camera cryptoCam; // camera being used to take pictures


    public Image displayIm;
    public Texture2D test;
    public Text testtxt;

    [SerializeField]
    Text picText;

    [SerializeField]
    GameObject cameraOverlay;

    [SerializeField]
    CameraSnap cameraSnap;

    //tentative method for subject- et list of all cryptids and check if visible in frame
    GameObject[] allCryptids;

    //save field of view for non zoom;
    float defaultFOV;

    //state for whether player is preparing to take a photo
    public bool CameraReady;

    [SerializeField]
    AudioSource cameraSFX;

	// Use this for initialization
	void Start () {
        picIndex = 0;
        allPics = new Photograph[maxPics];
        allCryptids = GameObject.FindGameObjectsWithTag("Cryptid");
        picText.text = (allPics.Length - picIndex).ToString();
        defaultFOV = cryptoCam.fieldOfView;

    }
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetMouseButtonDown(0))
        if (CustomController.GetButtonDown(Constants.TakePicture))
        {
            if (cameraSnap == null || CameraReady == false) { return; }
            cameraSnap.SnapShutter();
            TakePicture();
            if (cameraSFX != null) { cameraSFX.Play(); }
            if (picText != null) picText.text = (allPics.Length - picIndex).ToString();
        }
        if (CustomController.GetButtonDown(Constants.ReadyCamera))
        {
            ReadyCamera();
        }
        if (CustomController.GetButtonUp(Constants.ReadyCamera))
        {
            UnReadyCamera();
        }
 
	}

    //ready camera by zooming and bringing up camera overlay
    void ReadyCamera()
    {
        cameraOverlay.SetActive(true);
        cryptoCam.fieldOfView = defaultFOV / 2;
        CameraReady = true;
    }

    //return to default camera state
    void UnReadyCamera()
    {
        cameraOverlay.SetActive(false);
        cryptoCam.fieldOfView = defaultFOV;
        CameraReady = false;
    }

    //rendertexture (photo) is saved. this is also where some grading happens
    void TakePicture()
    {

        if (picIndex >= allPics.Length)
        {
            this.GetComponent<GameManager>().EndPrompt();
            return;
        }

        //take the photo
        //https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html
        RenderTexture rt = new RenderTexture(cryptoCam.pixelWidth, cryptoCam.pixelHeight, 24);
        cryptoCam.targetTexture = rt;
        Texture2D screenshot = new Texture2D(cryptoCam.pixelWidth, cryptoCam.pixelHeight, TextureFormat.RGB24, true);
        cryptoCam.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, cryptoCam.pixelWidth, cryptoCam.pixelHeight), 0, 0);
        screenshot.Apply();
        cryptoCam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        //create the photo
        Photograph pic = new Photograph();
        pic.pic = screenshot;
        
        //Display(screenshot);

        //determine who's in the photo
        List<GameObject> subjects = new List<GameObject>();
        GameObject mainSubject;
        foreach (GameObject cryptid in allCryptids)
        {
            //https://answers.unity.com/questions/8003/how-can-i-know-if-a-gameobject-is-seen-by-a-partic.html
            Renderer renderer = cryptid.GetComponent<Cryptid>().renderer;
            //check if cyrptid position is visible by camera
            if (renderer != null && renderer.isVisible)
            {
                Vector3 viewPos = cryptoCam.WorldToViewportPoint(cryptid.transform.position);
                if ((viewPos.x >= 0) && (viewPos.x <= 1) && (viewPos.y >= 0) && (viewPos.y <= 1) && (viewPos.z > 0))
                {
                    //if x and y are between 1 and 0 ((0,0) is bottom left corner and (1,1) is top riht) and z (distance from camera) is positive then cryptid is in the shot
                    //dont add cryptids that are in frame but not visible
                    if (checkVisibility(cryptid) != 0) { subjects.Add(cryptid); }
                }
            }
        }
        pic.subjectCount = subjects.Count;

        //automatic 0 if no one is in the photo
        if (subjects.Count == 0)
        {
            pic.baseScore = 0;
            pic.subjectName = "No one";
            //store the pic
            allPics[picIndex] = pic;

            //Display(pic.pic);
            //DisplayScore(0);

            //move up index
            picIndex++;

            if (picIndex >= allPics.Length)
            {
                this.GetComponent<GameManager>().EndPrompt();
            }

            return;
        }

        //else if(subjects.Count == 1)
        //treatin this as default case to avoid issue of mainsubject being unnasigned
        //{

            //allScores[picIndex] = ScoreSubject(subjects[0]);
            mainSubject = subjects[0];
            pic.baseScore = mainSubject.GetComponent<Cryptid>().baseScore;
            pic.subjectName = mainSubject.GetComponent<Cryptid>().cryptidType;

        //}
        //if more than one cryptid is in the photo determine whos the subject
        //factors to consider: closer to camera (viewpos.z), closer to center of frame, more visible
        if (subjects.Count > 1)
        {
            float subjectScore = 1000000f;
            foreach (GameObject cryptid in subjects)
            {
                //viewpos.z represents distance from camera (z=0 is on top of camera)
                //(.5,.5) is the center of the screen: |(x,y)-(.5,.5)| represents distance from center
                Vector3 viewPos = cryptoCam.WorldToViewportPoint(cryptid.transform.position);

                //calculate a "score" for each cryptid based on placement and visibility
                //distance from center should have more "weight" in score than distance from camera which is why its bein multipied
                Vector2 distanceFromCenter = new Vector2(.5f, .5f) - new Vector2(viewPos.x, viewPos.y);
                float currentScore = viewPos.z + (100 * distanceFromCenter.magnitude);

                //lowest scorin cryptid becomes the main subject: lower score means less distance from ideal placement
                if (currentScore < subjectScore)
                {
                    mainSubject = cryptid;
                    subjectScore = currentScore;
                }
            }

            pic.baseScore = mainSubject.GetComponent<Cryptid>().baseScore;
            pic.subjectName = mainSubject.GetComponent<Cryptid>().cryptidType;
        }

        //once main subject is determined check other score criteria

        //check if facing forward
        Vector3 cryptidForward = mainSubject.transform.forward;

        //if cryptid has a "head bone" (object that represents the head and face) then use the forward of that for a more accurate estimate of which way its facing
        HeadBone cryptidHead = mainSubject.GetComponentInChildren<HeadBone>();
        if (cryptidHead != null)
        {
            cryptidForward = cryptidHead.GetForward();
        }

        //if the cos of the angle between the cameras forward vector and the cryptids forward vector is less than 0,
        //then the vectors are in opposite directions and the cryptid is facing the camera
        if (Vector3.Dot(cryptoCam.transform.forward, cryptidForward) < 0)
        {
            pic.facinForward = true;
        }

        //todo: check for cool animation
        pic.coolPose = mainSubject.GetComponent<Cryptid>().SpecialPose();

        //store distance from center and distance from camera
        Vector3 cameraPos = cryptoCam.WorldToViewportPoint(mainSubject.transform.position);
        //if cryptid has a head bone use that as the center instead
        if (cryptidHead != null) { cameraPos = cryptoCam.WorldToViewportPoint(cryptidHead.transform.position); }

        pic.distanceFromCamera = cameraPos.z;
        Vector2 dfc = new Vector2(.5f, .5f) - new Vector2(cameraPos.x, cameraPos.y);
        pic.distanceFromCenter = dfc.magnitude;

        //store visibility
        pic.visibility = checkVisibility(mainSubject);

        //score the pic
        pic.finalScore = ScorePhoto(pic);

        //store the pic
        allPics[picIndex] = pic;

        //display (debu)
        //Display(pic.pic);
        //ScorePhoto(pic);
        //DisplayScore(ScorePhoto(pic));
        

        //displaying all elements of score
        //testtxt.text = "Score: " + ScorePhoto(pic);
        //testtxt.text += '\n' + "Cryptids in Pic: " + pic.subjectCount;
        //testtxt.text += '\n' + "Subject: " + pic.subjectName;
        //testtxt.text += '\n' + "Facin forward: " + pic.facinForward;
        //testtxt.text += '\n' + "Visibility: " + pic.visibility;

        //move up index
        picIndex++;

        //check if we're allowed to take any more photos
        if (picIndex >= allPics.Length)
        {
            this.GetComponent<GameManager>().EndPrompt();
            return;
        }
    }



    int ScorePhoto(Photograph pic)
    {
        //in the future this will scroll throuh dialoue, but for now it just returns a score
        int finalScore = 0;

        //oh, its a TYPE! thats worth BASESCORE points
        testtxt.text = "Subject: " + pic.subjectName + " (+" + pic.baseScore + ")";
        finalScore += pic.baseScore;


        //base score reduced to 10 points if facing wrong way
        testtxt.text += '\n' + "Facing forward: " + pic.facinForward;
        if (!pic.facinForward)
        {
            finalScore -= (pic.baseScore - 10);
            testtxt.text += " (-" + (pic.baseScore - 10) + ")";
        }

        //todo: visibility
        finalScore = (int)(finalScore*pic.visibility);
        testtxt.text += '\n' + "Visibility: " + (int)(pic.visibility * 100) + "% (x" + pic.visibility + ")";

        //todo: coolpose
        if (pic.coolPose)
        {
            int coolPoseBonus = 200;
            finalScore += coolPoseBonus;
        }

        //distancefromcenter should be a float value between 0 and ~.7
        finalScore += (int)(200 * (.7f - pic.distanceFromCenter));
        testtxt.text += "\n" + "Distance from Center of Frame: \n" + pic.distanceFromCenter + " (+" + (int)(200 * (.7f - pic.distanceFromCenter)) + ")";

        //distance from camera is in world units, with 0 on top of camera
        //not sure what ideal distance is, for now well just say the closer the better
        //distance from camera should never be 0, but just in case
        if (pic.distanceFromCamera == 0) { pic.distanceFromCamera = .000001f; }
        finalScore += (int)Mathf.Ceil(5000 * (1 / pic.distanceFromCamera));
        testtxt.text += "\n" + "Distance from Camera: \n" + pic.distanceFromCamera + "(+" + (int)Mathf.Ceil(5000 * (1 / pic.distanceFromCamera)) + ")";


        //and theres more than one cryptid in the shot! thats worth double!
        if (pic.subjectCount > 1)
        {
            finalScore *= 2;
        }
        testtxt.text += "\nCryptids in Frame: " + pic.subjectCount + " (x" + pic.subjectCount + ")";

        testtxt.text += '\n' + "Score: " + finalScore;

        return finalScore;
    }

    float checkVisibility(GameObject mainSubject)
    {
        //todo: check for visibility

        //if cryptid has set itself to invisible use that
        if (!mainSubject.GetComponent<Cryptid>().IsVisible()) { return 0; }

        //cycle throuh hitboxes on cryptid, raycastin to each one
        //pic.visibility is hitboxes successfully hit by raycast/total hitboxes
        Collider[] hitboxes = mainSubject.GetComponentsInChildren<Collider>();
        int hitCounter = 0;
        foreach (Collider check in hitboxes)
        {
            Vector3 direction = check.transform.position - cryptoCam.transform.position;
            Ray ray = new Ray(cryptoCam.transform.position, direction);
           
            RaycastHit hit;

            // Bit shift the index of the layer (1 - transparent fx) to get a bit mask
            int layerMask = 1 << 1;

            // This would cast rays only against colliders in layer 1.
            // But instead we want to collide against everything except layer 1. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                //if (hit.collider == check)
                //rather than checkin if its the exact same collider, check if they have the same parent
                //this avoids issues where the cryptids head blocks other parts of its body
                if (hit.collider.transform.root == check.transform.root)
                {
                    hitCounter++;
                }
                else if (check.transform.tag == "Optional")
                {
                    hitCounter++;
                }
                else
                {
                    Debug.Log("obstacle hit: " + hit.transform.ToString());
                    Debug.Log("attempting to hit: " + check.transform.ToString());
                    Debug.DrawRay(cryptoCam.transform.position, direction, Color.yellow, 10);
                }
            }      
        }
        if (hitboxes.Length != 0) { return (float)hitCounter / (float)hitboxes.Length; }

        return 0;
    }

    public Photograph[] GetPhotographs()
    {
        return allPics;
    }

   
}
