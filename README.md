# NEW: Experimental Package Manager support for Unity 2018.3 and higher
You can now install this using the built-in package manager. You only need to perform the following steps (you need to repeat these steps each time you create a new project):
  * Create your project
  * Find the Packages/Manifest.json file in the root folder of your project
  * Add the scopedRegistry section to the bottom of the manifest (dont forget the , after the dependencies }) 
<pre>
 {
  "dependencies": {
    ...
  }<font color="green">,
  "scopedRegistries": [
    {
      "name": "HKUECT",
      "url": "http://37.97.171.71:4873/",
      "scopes": [
        "nl.hku"
      ]
    }
  ]</font>
}
</pre>
  * Restart Unity, and open the Window -> Package Manager
  * You will need to select "display preview packages" for this package

# Interactive360
Custom version of Unity interactive 360 example for use with monoscopic 360 content for VR.

Based on Unity's own example code: https://blogs.unity3d.com/2018/01/19/getting-started-in-interactive-360-video-download-our-sample-project/

Changes include:
  - Additive loading of scenes to prevent delays when transitioning
  - Users can work on a scene and link scenes together more easily, without having to jump around
  - Support for 360 photos with PhotoSpheres
  - VideoPlayer preview controls for easy hotspot placement

Current Work:
  - Finish Guide (see below), + translate to English
  - User test + fix crucial functionality / usability issues

Future work:
  - Clean up the currently hacky implementation
  - Add support for animation / sound cues from hotspot interactions

Guide: https://docs.google.com/document/d/1OA4tkpSlE70pcxec8rRz_iIMjKGZZEtIRvh-9nvgShg/edit?usp=sharing
