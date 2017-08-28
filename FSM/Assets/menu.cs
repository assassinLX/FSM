using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class menu : EditorWindow {

    [MenuItem("Menu/Clone GameObject")]
    static void ClothObject()
    {
        MonoBehaviour.Instantiate(Selection.activeTransform, Vector3.zero, Quaternion.identity);
    }

    
    [MenuItem("Menu/Clone GameObject",true)]
    static bool NoClothObject()
    {
        return Selection.activeGameObject != null;
    }
   
    
    [MenuItem("Menu/Delete Object")]
    static void RemoveObject()
    {
        MonoBehaviour.DestroyImmediate(Selection.activeGameObject, true);
    }

    [MenuItem("Menu/Delete Object", true)]
    static bool NoRemoveObject()
    {
        return Selection.activeGameObject != null;
    }
    
    [MenuItem("Menu/Creat State")]
    static void CreatMyStateFile()
    {
        menu win = (menu)EditorWindow.GetWindow(typeof(menu));
        win.Show();
    }


    string InputString;
    int StateNumber;
    string [] stateName;
    string FloderName = "FiniteStateMachine";
    string interfaceName = "MyInterface";
    string FSMControllerName = "FSMController";

    private void OnGUI()
    {
        GUILayout.Label("Current State", EditorStyles.boldLabel);
        InputString = EditorGUILayout.TextField("状态个数", InputString);
        if(int.TryParse(InputString,out StateNumber))
            {
                if(stateName == null)
                {
                stateName = new string[StateNumber];
                }
                for (int i = 0; i < StateNumber; i++)
                {
                    stateName[i] = EditorGUILayout.TextField("状态" + i, stateName[i]);
                }
                FloderName = EditorGUILayout.TextField("文件夹的名字", FloderName);
                interfaceName = EditorGUILayout.TextField("接口名字",interfaceName);
                FSMControllerName = EditorGUILayout.TextField("中心控制器的名字",FSMControllerName);

                if (GUILayout.Button("生成"))
                {
                    AssetDatabase.CreateFolder("Assets",FloderName);
                    //1 创建一个接口 第一版 有三个接口
                    string path = "Assets/" + FloderName + "/"+ interfaceName + ".cs";
                
                    if(File.Exists(path) == false){
                        using (StreamWriter outfile = new StreamWriter(path)){
                            outfile.WriteLine("using UnityEngine;");
                            outfile.WriteLine("using System.Collections;");
                            outfile.WriteLine("");
                            outfile.WriteLine("public interface "+ interfaceName +" {");
                            outfile.WriteLine(" ");
                            outfile.WriteLine("        void _Start(GameObject self);");
                            outfile.WriteLine(" ");
                            outfile.WriteLine("        void _Update();");
                            outfile.WriteLine(" ");
                            outfile.WriteLine("        void _Over();");
                            outfile.WriteLine(" ");
                            outfile.WriteLine("        //...有需要可以增加");
                            outfile.WriteLine("}");
                        }
                    }

                    //2 创建继承接口的文件的文件夹 创建继承接口的文件 
                    string detailsName = "details";
                    string detailsPath = "Assets/" + FloderName;
                    AssetDatabase.CreateFolder(detailsPath,detailsName);

                    foreach(string name in stateName){
                        string detailsFilePath = detailsPath + "/" + detailsName + "/" + name + ".cs";
                        
                        if(File.Exists(detailsFilePath) == false){
                            using (StreamWriter outfile = new StreamWriter(detailsFilePath)){
                                outfile.WriteLine("using UnityEngine;");
                                outfile.WriteLine("using System.Collections;");
                                outfile.WriteLine("");
                                outfile.WriteLine("public class "+ name +" : "+ interfaceName +"{");
                                outfile.WriteLine(" ");
                                outfile.WriteLine("        public GameObject self;");
                                outfile.WriteLine(" ");
                                outfile.WriteLine("        public void _Start(GameObject self){\n       this.self = self;\n       }");
                                outfile.WriteLine(" ");
                                outfile.WriteLine("        public void _Update(){\n       }");
                                outfile.WriteLine(" ");
                                outfile.WriteLine("        public void _Over(){\n        }");
                                outfile.WriteLine("}");
                            }
                        }
                    }        
                    
                    //3 建立controller 进行有限状态机的调用
                    string controllerPath = detailsPath + "/" + FSMControllerName + ".cs";

                    if(File.Exists(controllerPath) == false){
                        using (StreamWriter outfile = new StreamWriter(controllerPath)){
                            outfile.WriteLine("using UnityEngine;");
                            outfile.WriteLine("using System.Collections;");
                            outfile.WriteLine("");
                            outfile.WriteLine("public class "+ FSMControllerName +": Behaviour{");
                            outfile.WriteLine(" ");
                            outfile.WriteLine("        public enum State{");
                            foreach(string name in stateName){
                                outfile.WriteLine("         "+name + ",");
                            }
                            outfile.WriteLine("        }");

                            outfile.WriteLine("        public State _state;");
                            outfile.WriteLine(" ");
                            foreach(string name in stateName){
                                outfile.WriteLine("        public " + name + " _"+name +";\n");
                            }
                            outfile.WriteLine(" ");


                            outfile.WriteLine("      private void Start(){");
                            outfile.WriteLine("            _state = new State();");
                            
                            foreach(string name in stateName){
                                outfile.WriteLine("            _"+name+" = " + "new "+ name + "();");
                            }
                            outfile.WriteLine("      }");

                            
                            outfile.WriteLine("         private void Update(){");
                            outfile.WriteLine("         //...放入判断的条件 ");
                            outfile.WriteLine("            if(false){\n "+"                 _state" + " = "+" State."+stateName[0]+";\n            }");
                            for(int i = 1 ; i < stateName.Length ;i++){
                            outfile.WriteLine("            else if (false){\n "+"                  _state"+" = "+" State."+stateName[i]+";\n            }"); 
                            }
                            outfile.WriteLine(" ");
                            outfile.WriteLine(" ");

                            foreach(string name in stateName){
                                outfile.WriteLine("         if("+"_state"+ " == "+"State."+name + "){");
                                for(int i = 0 ; i < stateName.Length; i++){
                                    if(stateName[i] != name){
                                        outfile.WriteLine("                if(_"+stateName[i]+".self"+" != "+"null){");
                                        outfile.WriteLine("                    _"+stateName[i]+"._Over();");
                                        outfile.WriteLine("            }"); 
                                    }
                                }
                                outfile.WriteLine("                if(_"+name+" == null){");
                                outfile.WriteLine("                    _"+name+"._Start(transform.gameObject);");
                                outfile.WriteLine("            }");
                                outfile.WriteLine("                "+"_"+ name + "._Update();");
                                outfile.WriteLine("         }");
                            }
                            
                            outfile.WriteLine("    }");
                            outfile.WriteLine("}");
                        }
                    }


                    AssetDatabase.Refresh();
                    
                }

            }

        }




}
