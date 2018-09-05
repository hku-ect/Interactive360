using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using HKUECT;

//The Game Manager keeps track of which scenes to load, handles loading scenes, fading between scenes and also video playing/pausing

namespace Interactive360
{

    public class GameManager : MonoBehaviour
    {

        public static GameManager instance = null;

        Scene scene;
        VideoPlayer video;
        Animator anim;
        Image fadeImage;

        AsyncOperation operation;


        [Header("Scene Management")]
        public SceneReference[] scenesToLoad;
        public string activeScene;

        Scene? activeSc;

        [Space]
        [Header("UI Settings")]

        public bool useFade;
        public GameObject fadeOverlay;
        public GameObject ControlUI;
        public GameObject LoadingUI;
        public Image m_SelectionImage;
        public ulong globalFrame = 0;
        public float videoFrameRate = 30.0f;



        //make sure that we only have a single instance of the game manager
        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        IEnumerator GlobalFrameUpdate() {
            float frameTime = 1 / videoFrameRate;
            while( Application.isPlaying ) {
                globalFrame++;
                yield return new WaitForSeconds(frameTime);
            }
        }

        
        //set the initial active scene
        IEnumerator Start()
        {
            scene = SceneManager.GetActiveScene();
            activeScene = scene.buildIndex + " - " + scene.name;

            //load all scenes so we have everything setup
            //TODO: check if this works memory-wise with large video files etc.
            foreach( SceneReference sr in scenesToLoad ) {
                AsyncOperation op = SceneManager.LoadSceneAsync( sr, LoadSceneMode.Additive );
                yield return op;
            }

            //skip a frame before activation
            yield return null;

            SwitchSceneProperly(scenesToLoad[0]);

            StartCoroutine( GlobalFrameUpdate() );
        }

        public void SwitchSceneProperly(string newScene ) {
            Scene ns = SceneManager.GetSceneByName(newScene);
            StartCoroutine(HandleSceneActivation(ns));
        }

        IEnumerator HandleSceneActivation( Scene newScene ) { 
            if ( activeSc != null ) {
                Deactivate( activeSc.Value );
            }

            SceneManager.SetActiveScene(newScene);

            yield return null;
            
            Activate( newScene );

            activeSc = newScene;
        }


        void Deactivate( Scene sc ) {
            VideoPlayer vp;
            PhotoSphere ps;
            HotspotManager hsman;
            SceneObjects scObjs;

            //clean up (stop videoplayer etc.)
            if ( sc.FindInScene<VideoPlayer>(out vp ) ) {
                vp.Stop();
            }
            //hide hotspots
            if ( sc.FindInScene<HotspotManager>(out hsman) ) {
                hsman.Hide();
            }
            //hide photosphere's if there are any
            if ( sc.FindInScene<PhotoSphere>(out ps ) ) {
                ps.Hide();
            }
            //find and hide scene objects
            if ( sc.FindInScene<SceneObjects>(out scObjs ) ) {
                scObjs.Hide();
            }
        }

        void Activate( Scene sc ) {
            VideoPlayer vp;
            PhotoSphere ps;
            HotspotManager hsman;
            SceneObjects scObjs;

            //find videoplayer
            if ( sc.FindInScene<VideoPlayer>(out vp ) ) {
                vp.Play();
                if ( globalFrame > 0 && vp.clip.frameCount > 0 && vp.isLooping ) {
                    vp.frame = (long) ( globalFrame % vp.clip.frameCount );
                }
            }

            //find hotspotmanager and show hotspots
            if ( sc.FindInScene<HotspotManager>(out hsman) ) {
                hsman.Show();
            }

            //show photosphere if there is one
            //TODO: Figure out if we need to support multiple photospheres
            if ( sc.FindInScene<PhotoSphere>(out ps ) ) {
                ps.Show();
            }

            //find and show scene objects
            if ( sc.FindInScene<SceneObjects>(out scObjs ) ) {
                scObjs.Show();
            }
        }


        //Select scene is called from either the menu manager or hotspot manager, and is used to load the desired scene
        public void SelectScene(string sceneToLoad)
        {
            //if we want to use the fading between scenes, start the coroutine here
            if (useFade)
            {
                StartCoroutine(FadeOutAndIn(sceneToLoad));
            }
            //if we dont want to use fading, just load the next scene
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            //set the active scene to the next scene
            activeScene = sceneToLoad;
        }

        IEnumerator FadeOutAndIn(string sceneToLoad)
        {
            //get references to animatior and image component 
            anim = fadeOverlay.GetComponent<Animator>();
            fadeImage = fadeOverlay.GetComponent<Image>();

            //turn control UI off and loading UI on
            ControlUI.SetActive(false);
            LoadingUI.SetActive(true);

            //set FadeOut to true on the animator so our image will fade out
            anim.SetBool("FadeOut", true);

            //wait until the fade image is entirely black (alpha=1) then load next scene
            yield return new WaitUntil(() => fadeImage.color.a == 1);
            SceneManager.LoadScene(sceneToLoad);
            Scene scene = SceneManager.GetSceneByName(sceneToLoad);
            Debug.Log("loading scene:" + scene.name);
            yield return new WaitUntil(() => scene.isLoaded);

            // grab video and wait until it is loaded and prepared before starting the fade out
            video = FindObjectOfType<VideoPlayer>();
            yield return new WaitUntil(() => video.isPrepared);

            //set FadeOUt to false on the animator so our image will fade back in 
            anim.SetBool("FadeOut", false);
            
            //wait until the fade image is completely transparent (alpha = 0) and then turn loading UI off and control UI back on
            yield return new WaitUntil(() => fadeImage.color.a == 0);
            LoadingUI.SetActive(false);
            
            //if we have not destroyed the control UI, set it to active
            if (ControlUI) 
            ControlUI.SetActive(true);

            

        }

        //Find the video in the scene and pause it
        public void PauseVideo()
        {
            if (!video)
            {
                video = FindObjectOfType<VideoPlayer>();
            }
            video.Pause();
        }

        //Find the video in the scene and play it
        public void PlayVideo()
        {
            if (!video)
            {
                video = FindObjectOfType<VideoPlayer>();
            }
            video.Play();
        }
    }
}

