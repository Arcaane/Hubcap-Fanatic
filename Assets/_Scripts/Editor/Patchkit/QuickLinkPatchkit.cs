using System.Collections;
using System.Collections.Generic;
using PatchKit.UnityEditorExtension.Core;
using UnityEditor;
using UnityEngine;

namespace Patchkit {
    public static class QuickLinkPatchkit {
        
        [MenuItem("Tools/PatchKit/Quick Account Setup", false, 3)]
        public static void LinkPatchkitAccount() {
            var apiKey = new ApiKey("1ca99e5bb4d6ed9f999f25157b9e5302");
            Config.LinkAccount(apiKey);
        }
    }
}