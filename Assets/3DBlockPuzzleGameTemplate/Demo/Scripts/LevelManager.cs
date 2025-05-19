using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine.SceneManagement;

namespace BlockPuzzleGameTemplate
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Level Number")]
        public int LevelsQueLength;
        public int CurrentLevelCounter;
        public Transform LevelsQueParent;
        [Space(10)]
        [Header("TheLevels")]
        public List<string> Levels = new List<string>();
        public List<GameObject> LevelsQue = new List<GameObject>();
        public List<string> Non_LoopingLevels = new List<string>();
        public List<string> LoopingLevels = new List<string>();

        [Space(10)]
        [Header("TheRandomLevels")]
        int randomLevel;
        public List<int> RandomLevels = new List<int>();

        [Space(10)]
        [Header("Debug")]
        public int DebugLevel;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            DeactivateLevels();
            LevelStart_ConfigureLevelsIntoQue();
        }

        public void ClearLevels()
        {
            for (int i = 0; i < LevelsQue.Count; i++)
            {
                Destroy(LevelsQue[i]);
            }
            LevelsQue.Clear();
            LevelStart_ConfigureLevelsIntoQue();
        }

        public void ActivateLevels()
        {
            LevelsQueParent.gameObject.SetActive(true);
        }

        public void DeactivateLevels()
        {
            LevelsQueParent.gameObject.SetActive(false);
        }

        int GetLevel()
        {
            if (PlayerPrefs.HasKey("LEVEL"))
            {
                return PlayerPrefs.GetInt("LEVEL");
            }
            else
            {
                PlayerPrefs.SetInt("LEVEL", 0);
                return 0;
            }
        }

        void LevelStart_ConfigureLevelsIntoQue()
        {
            CurrentLevelCounter = GetLevel();

            for (int i = CurrentLevelCounter; i < LevelsQueLength + CurrentLevelCounter + 1; i++)
            {
                if (i < Levels.Count)
                {
                    SpawnALevelIntoQue(i);
                }
                else
                {
                    //Beyond the levels, spawn a random level
                    SpawnRandomLevelIntoQue();
                }
            }

            LevelsQue[0].gameObject.SetActive(true);
        }
        [ContextMenu("GetRandomLevel")]
        void testrandom()
        {
            Debug.Log(GetRandomLevel());
        }
        int GetRandomLevel()
        {
            int random = Random.Range(0, LoopingLevels.Count);
            while (RandomLevels.Contains(random))
            {
                random = Random.Range(0, LoopingLevels.Count);
            }
            RandomLevels.Add(random);
            if (RandomLevels.Count > 4)
            {
                RandomLevels.RemoveAt(0);
            }


            for (int i = 0; i < Levels.Count; i++)
            {
                if (Levels[i] == LoopingLevels[random])
                {
                    return i;
                }
            }


            return 0;
        }


        [ContextMenu("populate looping levels")]
        public void TestLevelLooper()
        {
            LoopingLevels.Clear();
            AddLoopinglevels(Levels);
        }
        void SpawnRandomLevelIntoQue()
        {
            int random = GetRandomLevel();
            SpawnALevelIntoQue(random);
        }

        void AddLoopinglevels(List<string> TheList)
        {
            //add levels
            foreach (string level in TheList)
            {
                LoopingLevels.Add(level);
            }
            RandomizeLoopingLevels();
        }

        void RandomizeLoopingLevels()
        {
            for (int i = 0; i < LoopingLevels.Count; i++)
            {
                for (int j = 0; j < Non_LoopingLevels.Count; j++)
                {
                    if (LoopingLevels[i] == Non_LoopingLevels[j])
                    {
                        LoopingLevels.RemoveAt(i);
                    }
                }
            }
        }

        void SpawnALevelIntoQue(int level_number)
        {
            string[] name_full = Levels[level_number].Split('=');
            string name = name_full[0].Trim();

            GameObject temp_fetch = Resources.Load("Levels/" + name) as GameObject;
            GameObject spawned_level = Instantiate(temp_fetch, LevelsQueParent);
            spawned_level.SetActive(false);
            spawned_level.name = name;
            LevelsQue.Add(spawned_level);
        }

        [ContextMenu("Force End Level")]
        public void EndLevelScreen()
        {
            CurrentLevelCounter++;
            PlayerPrefs.SetInt("LEVEL", CurrentLevelCounter);

            //Spawn Next Series of levels
            SpawnOneLevel();

            LevelsQue[1].gameObject.SetActive(true);
            LevelsQue[0].gameObject.SetActive(false);

            GameObject Del = LevelsQue[0];
            LevelsQue.Remove(Del);
            Destroy(Del);

            Debug.Log("nextLevel");
        }

        [ContextMenu("Spawn A level")]
        public void SpawnOneLevel()
        {
            int i = CurrentLevelCounter + LevelsQueLength;

            if (i < Levels.Count)
            {
                SpawnALevelIntoQue(i);
            }
            else
            {
                //Beyond the levels, spawn a random level
                SpawnRandomLevelIntoQue();
            }
        }

        public void RestartLevel()
        {
            //Respawn the level
            GameObject Obj_ = LevelsQue[0];
            Obj_.SetActive(false);

            //Fetch from resources
            string[] name_full = Obj_.name.Split('=');
            string name = name_full[0].Trim();

            GameObject temp_fetch = Resources.Load("Levels/" + name) as GameObject;
            GameObject spawned_level = Instantiate(temp_fetch, LevelsQueParent);
            LevelsQue[0] = spawned_level;
            spawned_level.name = Obj_.name;
            LevelsQue[0].SetActive(true);
            //Destroy Previous Object
            Destroy(Obj_);
        }

        //checkif if is firsttime
        public bool isFirstTimePlay()
        {
            if (GetLevel() <= 0)
            {
                return true;
            }
            return false;
        }

        [ContextMenu("Force Fake Level")]
        public void EnableFakeLevel()
        {
            PlayerPrefs.SetInt("LEVEL", DebugLevel);
        }

        [ContextMenu("Populate Levels")]
        public void PopulateLevels()
        {
            List<string> levels = new List<string>();
            int count = 0;
            while (count < 100)
            {
                string thename = "Level_" + count.ToString();
                GameObject temp_fetch = Resources.Load("Levels/" + thename) as GameObject;
                if (temp_fetch)
                {
                    levels.Add(thename);
                }
                count += 1;

            }
            Levels = levels;
#if UNITY_EDITOR
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
        }
    }
}