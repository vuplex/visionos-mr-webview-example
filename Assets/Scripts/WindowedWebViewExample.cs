using System.Threading.Tasks;
using UnityEngine;
using Unity.PolySpatial;
using Vuplex.WebView;

/// <summary>
/// Demonstrates how to use the VisionOSWebView.CreateInWindow() API to open a webview
/// in a native visionOS (SwiftUI) window.
/// </summary>
public class WindowedWebViewExample : MonoBehaviour {

    IWebView webView;

    async void Start() {
        
        #if UNITY_VISIONOS && !UNITY_EDITOR
            // When running on the visionOS device or simulator, use VisionOSWebView.CreateInWindow()
            // to open a webview in a native visionOS window:
            // https://developer.vuplex.com/webview/VisionOSWebView#CreateInWindow
            webView = await VisionOSWebView.CreateInWindow();
        #else
            // The VisionOSWebView.CreateInWindow() API isn't supported in the Editor, so when running in the Editor,
            // fall back to creating a WebViewPrefab to display in the scene.
            // https://developer.vuplex.com/webview/WebViewPrefab
            webView = await _createWebViewForEditor();
        #endif

        // Now that the IWebView is created, the app can use IWebView APIs like LoadUrl()
        // https://developer.vuplex.com/webview/IWebView
        webView.LoadUrl("https://www.google.com");

        // If either the Volume or the webview window is closed, then quit the application with Application.Quit().
        // This ensures that if the user re-opens the app from the visionOS home view, the app starts back up in the intended state.
        // Otherwise, the default on visionOS is that the application will remain running in the background unless
        // the user force quits it. If the app continues to run in the background and the user re-opens it from
        // the visionOS home view, then visionOS will only reopen the window that was open last (either the Volume or the webview
        // window, but not both). 
        var volumeCamera = FindObjectOfType<VolumeCamera>();
        volumeCamera.OnWindowEvent.AddListener(state => {
            if (state.WindowEvent == VolumeCamera.WindowEvent.Closed) {
                Application.Quit();
            }
        });
        #if UNITY_VISIONOS && !UNITY_EDITOR
            (webView as VisionOSWebView).WindowClosed += (sender, eventArgs) => {
                Application.Quit();
            };
        #endif
    }

    async Task<IWebView> _createWebViewForEditor() {

            var webViewPrefab = WebViewPrefab.Instantiate(0.6f, 0.4f);
            webViewPrefab.transform.parent = transform;
            webViewPrefab.transform.localPosition = new Vector3(0, 0.2f, 0.4f);
            webViewPrefab.transform.localEulerAngles = new Vector3(0, 180, 0);    
            await webViewPrefab.WaitUntilInitialized();
            return webViewPrefab.WebView;
    }
}
