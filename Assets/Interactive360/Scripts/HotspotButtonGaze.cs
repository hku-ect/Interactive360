using System.Collections;
using UnityEngine;
using Interactive360.Utils;
using UnityEngine.UI;
using HKUECT;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

//This class will invoke an OnClick event for the hotspot button as soon after the users gazes over it for a defined period of time


namespace Interactive360
{
    [RequireComponent(typeof(VRInteractiveItem))]
    [RequireComponent(typeof(BoxCollider))]

    public class HotspotButtonGaze : MonoBehaviour
    {

        public SceneReference targetScene;

        private Button m_Button;                        // The button we are going to call onClick for
        private bool isOver = false;                    // Bool value to let us know whether or not the gaze is over the button
        private VRInteractiveItem m_InteractiveItem;    // The interactable object for where the user should look to cause on "onClick" event.
        internal bool m_UsingFillImage = true;            // Are we using the fill UI for this

        //[SerializeField] private Image m_SelectionImage;     // Reference to the image who's fill amount is adjusted to display the bar. This will likely be attached to our Camera UI
        //[SerializeField] 
        //private bool m_HideOnStart = true;                                     // Whether or not the bar should be visible at the start.

        public float m_GazeTime = 2f;                   // The time we are waiting to complete the gaze interaction
        public bool visible = true;
        public bool permanent = true;
        public int startFrame = 0;
        public int endFrame = 9999;

        private Coroutine m_SelectionFillRoutine;       // Used to start and stop the filling coroutine based on input.
        //private bool m_RadialFilled;                    // Used to allow the coroutine to wait for the bar to fill.
        private bool m_IsSelectionRadialActive;         // Whether or not the bar is currently useable.



        private void Awake()
        {
            m_Button = GetComponent<Button>(); //Reference to Button component 
            m_InteractiveItem = GetComponent<VRInteractiveItem>(); //Reference to VRInteractiveItem Component

            if (!visible ) {
                //turn off the image component
                GetComponent<Image>().enabled = false;
            }
        }

        void HandleColliderCoroutine( Scene oldScene, Scene newScene ) {
            if ( permanent ) return;

            if ( !newScene.IsInScene(transform)) {
                StopAllCoroutines();
            }
            else {
                StartCoroutine(HandleColliderActive());
            }
        }

        IEnumerator HandleColliderActive() {
            //Debug.Log("Ding");

            VideoPlayer pl;
            Collider c = GetComponent<Collider>();
            Scene sc = SceneManager.GetActiveScene();
            
            if ( sc.FindInScene<VideoPlayer>(out pl) ) {
                Image img = GetComponent<Image>();
                if ( startFrame > 0 ) {
                    img.enabled = false;
                }

                while ( Application.isPlaying ) {
                    bool shouldBeActive = ( pl.frame >= startFrame && pl.frame <= endFrame );

                    if ( shouldBeActive && !c.enabled ) {
                        if ( visible ) {
                            img.enabled = true;
                        }
                        c.enabled = true;
                    }
                    else if ( !shouldBeActive && c.enabled ) {
                        img.enabled = false;
                        c.enabled = false;
                    }

                    yield return null;
                }
            }

            yield return null;
        }

        private void Start()
        {
            // Setup the radial to have no fill at the start and hide if necessary.
            //m_SelectionImage.fillAmount = 0f;

            //if (m_HideOnStart)
            Hide();
        }

        public void Hide()
        {
            //m_SelectionImage.gameObject.SetActive(false);
            m_IsSelectionRadialActive = false;

            // This effectively resets the radial for when it's shown again.
            //m_SelectionImage.fillAmount = 0f;
        }

        public void Show()
        {
            //m_SelectionImage.gameObject.SetActive(true);
            m_IsSelectionRadialActive = true;
        }



        private void OnEnable()
        {
            m_InteractiveItem.OnOver += HandleOver;
            m_InteractiveItem.OnOut += HandleOut;

            
            if ( !permanent ) {
                StartCoroutine(HandleColliderActive());
                SceneManager.activeSceneChanged += HandleColliderCoroutine;
            }
        }


        private void OnDisable()
        {
            m_InteractiveItem.OnOver -= HandleOver;
            m_InteractiveItem.OnOut -= HandleOut;

            if ( !permanent ) {
                StopAllCoroutines();
                SceneManager.activeSceneChanged -= HandleColliderCoroutine;
            }
        }

        private IEnumerator FillSelectionRadial()
        {
            // At the start of the coroutine, the bar is not filled.
            //m_RadialFilled = false;


            // Make sure the radial is visible and usable.
            Show();

            // Create a timer and reset the fill amount.
            float timer = 0f;
            GameManager.instance.m_SelectionImage.fillAmount = 0f;

            // This loop is executed once per frame until the timer exceeds the duration.
            while (timer < m_GazeTime)
            {
                // The image's fill amount requires a value from 0 to 1 so we normalise the time.
                GameManager.instance.m_SelectionImage.fillAmount = timer / m_GazeTime;

                // Increase the timer by the time between frames and wait for the next frame.
                timer += Time.deltaTime;
                yield return null;
            }

            // When the loop is finished set the fill amount to be full.
            GameManager.instance.m_SelectionImage.fillAmount = 1f;

            // Turn off the radial so it can only be used once.
            m_IsSelectionRadialActive = false;

            // The radial is now filled so the coroutine waiting for it can continue.
            //m_RadialFilled = true;

            // call OnClick now that the selection is complete
            //m_Button.onClick.Invoke();
            GameManager.instance.SwitchSceneProperly(targetScene);

            // Once it's been used make the radial invisible.
            Hide();

        }


        // When the user looks at the rendering of the scene, wait 1 second
        // if we are still over after 1 full second, invoke click
        private IEnumerator WaitAndClick()
        {
            yield return new WaitForSeconds(m_GazeTime);
            if (isOver == true)
            {
                m_Button.onClick.Invoke();
            }

        }

        private void HandleOver()
        {
            isOver = true;

            //if this menu button gaze is for a hotspot, start the FillSelectionRadial coroutine. If it is not 
            if (m_UsingFillImage)
            {
                //Debug.Log("start coroutine");
                m_SelectionFillRoutine = StartCoroutine(FillSelectionRadial());
            }
            else
                StartCoroutine(WaitAndClick());
        }

        private void HandleOut()
        {
            isOver = false;

            // If the radial is active stop filling it and reset it's amount.
            if (m_IsSelectionRadialActive)
            {
                if (m_SelectionFillRoutine != null)
                    StopCoroutine(m_SelectionFillRoutine);

                GameManager.instance.m_SelectionImage.fillAmount = 0f;
            }
        }



    }
}
