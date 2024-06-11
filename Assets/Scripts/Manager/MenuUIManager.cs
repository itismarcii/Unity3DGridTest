using System;
using UI.Menu;
using UI.WorldScene;
using UnityEngine;

namespace Manager
{
    public class MenuUIManager : MonoBehaviour
    {
        public static WorldCreatorUI WorldCreatorUI;
        public static WorldSceneUI WorldSceneUI;

        public Transform GetBuildingFrame() => WorldSceneUI._BuildingListFrame;
    }
}
