using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using LastSignal.Core;
using LastSignal.Data;

namespace LastSignal.Editor
{
    public class AutoSetupPendingTasks : EditorWindow
    {
        [MenuItem("LastSignal/Executar Auto-Setup das Pendências")]
        public static void RunSetup()
        {
            Debug.Log("Iniciando Auto-Setup...");

            // SetupAudioManager(); // Já configurado
            // SetupEventManager(); // O usuário já configurou manualmente!
            // SetupEventManager(); // O usuário já configurou manualmente!
            CreateGameOverUI();
            CreateMainMenuAndBuildSettings();
            
            Debug.Log("Auto-Setup Concluído com Sucesso!");
        }

        // Métodos de áudio removidos pois geravam erro de compilação em campos privados

        private static void SetupEventManager()
        {
            var eventManager = Object.FindObjectOfType<EventManager>(true);
            if (eventManager == null)
            {
                Debug.LogWarning("EventManager não encontrado na cena!");
                return;
            }

            List<EventEntry> newEvents = new List<EventEntry>();

            newEvents.Add(CreateEvent("MSG_Day_02", 2, 2, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_03", 3, 3, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_04", 4, 4, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_05", 5, 5, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_07", 7, 7, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_08", 8, 8, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_09", 9, 9, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_11", 11, 11, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_12", 12, 12, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_13", 13, 13, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Day_14", 14, 14, false, CommodityType.Food, ResourceComparison.LessThan, 0));

            newEvents.Add(CreateEvent("MSG_Mil_01", 2, 5, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Mil_02", 6, 9, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Mil_03", 10, 14, false, CommodityType.Food, ResourceComparison.LessThan, 0));

            newEvents.Add(CreateEvent("MSG_Env_01", 3, 5, true, CommodityType.Water, ResourceComparison.LessThan, 30));
            newEvents.Add(CreateEvent("MSG_Env_02", 6, 9, true, CommodityType.Fuel, ResourceComparison.LessThan, 20));
            newEvents.Add(CreateEvent("MSG_Env_03", 10, 14, false, CommodityType.Food, ResourceComparison.LessThan, 0));

            newEvents.Add(CreateEvent("MSG_Myst_01", 2, 4, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Myst_02", 6, 10, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Myst_03", 11, 14, false, CommodityType.Food, ResourceComparison.LessThan, 0));

            newEvents.Add(CreateEvent("MSG_Int_01", 4, 6, true, CommodityType.Food, ResourceComparison.LessThan, 40));
            newEvents.Add(CreateEvent("MSG_Int_02", 7, 10, false, CommodityType.Food, ResourceComparison.LessThan, 0));
            newEvents.Add(CreateEvent("MSG_Int_03", 11, 14, false, CommodityType.Food, ResourceComparison.LessThan, 0));

            var serializedObject = new SerializedObject(eventManager);
            serializedObject.Update();
            
            var eventsProp = serializedObject.FindProperty("events");
            eventsProp.arraySize = newEvents.Count;

            for (int i = 0; i < newEvents.Count; i++)
            {
                var element = eventsProp.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("id").stringValue = newEvents[i].id;
                element.FindPropertyRelative("message").objectReferenceValue = newEvents[i].message;
                element.FindPropertyRelative("triggerOnce").boolValue = newEvents[i].triggerOnce;
                
                var cond = element.FindPropertyRelative("condition");
                cond.FindPropertyRelative("evaluateOn").enumValueIndex = (int)newEvents[i].condition.evaluateOn;
                cond.FindPropertyRelative("minDay").intValue = newEvents[i].condition.minDay;
                cond.FindPropertyRelative("maxDay").intValue = newEvents[i].condition.maxDay;
                cond.FindPropertyRelative("checkResource").boolValue = newEvents[i].condition.checkResource;
                cond.FindPropertyRelative("resourceType").enumValueIndex = (int)newEvents[i].condition.resourceType;
                cond.FindPropertyRelative("comparison").enumValueIndex = (int)newEvents[i].condition.comparison;
                cond.FindPropertyRelative("threshold").intValue = newEvents[i].condition.threshold;
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(eventManager);
            Debug.Log("EventManager populado com 23 eventos.");
        }

        private static EventEntry CreateEvent(string id, int min, int max, bool checkRes, CommodityType resType, ResourceComparison comp, int thresh)
        {
            var entry = new EventEntry();
            entry.id = id;
            entry.triggerOnce = true;
            entry.condition = new EventCondition
            {
                evaluateOn = EvaluationTrigger.DayChange,
                minDay = min,
                maxDay = max,
                checkResource = checkRes,
                resourceType = resType,
                comparison = comp,
                threshold = thresh
            };

            string[] guids = AssetDatabase.FindAssets(id + " t:MessageData");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                entry.message = AssetDatabase.LoadAssetAtPath<MessageData>(path);
            }
            else
            {
                Debug.LogWarning("MessageData não encontrado para " + id);
            }

            return entry;
        }

        private static void CreateGameOverUI()
        {
            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null) return;

            Transform existing = canvas.transform.Find("GameOverScreen");
            if (existing != null)
            {
                Debug.Log("GameOverScreen já existe.");
                return;
            }

            GameObject goScreen = new GameObject("GameOverScreen");
            goScreen.transform.SetParent(canvas.transform, false);
            
            var rt = goScreen.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var cg = goScreen.AddComponent<CanvasGroup>();

            var img = goScreen.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0, 0, 0, 0.95f);

            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(goScreen.transform, false);
            var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
            titleTxt.text = "SINAL PERDIDO.";
            titleTxt.fontSize = 48;
            titleTxt.color = Color.red;
            titleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            var trt = titleObj.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 0.6f);
            trt.anchorMax = new Vector2(1, 0.8f);
            trt.offsetMin = Vector2.zero;
            trt.offsetMax = Vector2.zero;

            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(goScreen.transform, false);
            var descTxt = descObj.AddComponent<TMPro.TextMeshProUGUI>();
            descTxt.text = "O bunker colapsou.";
            descTxt.fontSize = 24;
            descTxt.color = Color.white;
            descTxt.alignment = TMPro.TextAlignmentOptions.Center;
            var drt = descObj.GetComponent<RectTransform>();
            drt.anchorMin = new Vector2(0.1f, 0.4f);
            drt.anchorMax = new Vector2(0.9f, 0.6f);
            drt.offsetMin = Vector2.zero;
            drt.offsetMax = Vector2.zero;

            GameObject btnObj = new GameObject("MenuButton");
            btnObj.transform.SetParent(goScreen.transform, false);
            var btnImg = btnObj.AddComponent<UnityEngine.UI.Image>();
            btnImg.color = Color.gray;
            var btn = btnObj.AddComponent<UnityEngine.UI.Button>();
            var brt = btnObj.GetComponent<RectTransform>();
            brt.anchorMin = new Vector2(0.4f, 0.2f);
            brt.anchorMax = new Vector2(0.6f, 0.3f);
            brt.offsetMin = Vector2.zero;
            brt.offsetMax = Vector2.zero;

            GameObject btnTextObj = new GameObject("Text");
            btnTextObj.transform.SetParent(btnObj.transform, false);
            var btnTxt = btnTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            btnTxt.text = "VOLTAR AO MENU";
            btnTxt.fontSize = 20;
            btnTxt.color = Color.black;
            btnTxt.alignment = TMPro.TextAlignmentOptions.Center;
            var btrt = btnTextObj.GetComponent<RectTransform>();
            btrt.anchorMin = Vector2.zero;
            btrt.anchorMax = Vector2.one;
            btrt.offsetMin = Vector2.zero;
            btrt.offsetMax = Vector2.zero;

            var gos = goScreen.AddComponent<LastSignal.UI.GameOverScreen>();
            var gosSo = new SerializedObject(gos);
            gosSo.Update();
            gosSo.FindProperty("canvasGroup").objectReferenceValue = cg;
            gosSo.FindProperty("titleLabel").objectReferenceValue = titleTxt;
            gosSo.FindProperty("messageLabel").objectReferenceValue = descTxt;
            gosSo.FindProperty("menuButton").objectReferenceValue = btn;
            gosSo.ApplyModifiedProperties();

            EditorUtility.SetDirty(gos);
            goScreen.SetActive(false);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("GameOverScreen criado.");
        }

        private static void CreateMainMenuAndBuildSettings()
        {
            string menuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
            string sampleScenePath = "Assets/_Project/Scenes/SampleScene.unity";

            if (!System.IO.File.Exists(menuScenePath))
            {
                // Create directory if not exists
                if (!System.IO.Directory.Exists("Assets/_Project/Scenes"))
                    System.IO.Directory.CreateDirectory("Assets/_Project/Scenes");

                var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                
                GameObject cameraObj = new GameObject("Main Camera");
                var cam = cameraObj.AddComponent<Camera>();
                cam.backgroundColor = Color.black;
                cam.clearFlags = CameraClearFlags.SolidColor;

                GameObject canvasObj = new GameObject("Canvas");
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

                GameObject titleObj = new GameObject("Title");
                titleObj.transform.SetParent(canvasObj.transform, false);
                var titleTxt = titleObj.AddComponent<TMPro.TextMeshProUGUI>();
                titleTxt.text = "LAST SIGNAL\n<size=24>BUNKER ALFA</size>";
                titleTxt.fontSize = 64;
                titleTxt.color = new Color(0.3f, 1f, 0.3f);
                titleTxt.alignment = TMPro.TextAlignmentOptions.Center;
                var trt = titleObj.GetComponent<RectTransform>();
                trt.anchorMin = new Vector2(0, 0.6f);
                trt.anchorMax = new Vector2(1, 0.9f);
                trt.offsetMin = Vector2.zero;
                trt.offsetMax = Vector2.zero;

                GameObject btnObj = new GameObject("PlayButton");
                btnObj.transform.SetParent(canvasObj.transform, false);
                var btnImg = btnObj.AddComponent<UnityEngine.UI.Image>();
                btnImg.color = new Color(0.1f, 0.3f, 0.1f);
                var btn = btnObj.AddComponent<UnityEngine.UI.Button>();
                var brt = btnObj.GetComponent<RectTransform>();
                brt.anchorMin = new Vector2(0.4f, 0.4f);
                brt.anchorMax = new Vector2(0.6f, 0.5f);
                brt.offsetMin = Vector2.zero;
                brt.offsetMax = Vector2.zero;

                GameObject btnTextObj = new GameObject("Text");
                btnTextObj.transform.SetParent(btnObj.transform, false);
                var btnTxt = btnTextObj.AddComponent<TMPro.TextMeshProUGUI>();
                btnTxt.text = "> INICIAR CONEXÃO";
                btnTxt.fontSize = 20;
                btnTxt.color = Color.white;
                btnTxt.alignment = TMPro.TextAlignmentOptions.Center;
                var btrt = btnTextObj.GetComponent<RectTransform>();
                btrt.anchorMin = Vector2.zero;
                btrt.anchorMax = Vector2.one;
                btrt.offsetMin = Vector2.zero;
                btrt.offsetMax = Vector2.zero;
                
                // UnityEvent hook
                UnityEditor.Events.UnityEventTools.AddStringPersistentListener(
                    btn.onClick,
                    new UnityEngine.Events.UnityAction<string>(SceneManager.LoadScene),
                    "SampleScene"
                );

                EditorSceneManager.SaveScene(newScene, menuScenePath);
                Debug.Log("Cena MainMenu criada e salva.");
            }

            // Build settings
            var scenes = new List<EditorBuildSettingsScene>();
            scenes.Add(new EditorBuildSettingsScene(menuScenePath, true));
            
            // Tenta adicionar a SampleScene
            string sampleSceneGuid = AssetDatabase.AssetPathToGUID(sampleScenePath);
            if (!string.IsNullOrEmpty(sampleSceneGuid))
            {
                scenes.Add(new EditorBuildSettingsScene(sampleScenePath, true));
            }
            else
            {
                string alternativePath = "Assets/Scenes/SampleScene.unity";
                if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(alternativePath)))
                {
                    scenes.Add(new EditorBuildSettingsScene(alternativePath, true));
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("Build Settings configurado com MainMenu e SampleScene.");

            // Volta pra SampleScene
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            // Se falhar
            // Tenta abrir de onde estiver.
        }
    }
}
