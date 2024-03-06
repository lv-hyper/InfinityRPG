using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace InGame.UI
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Board Section Data", menuName = "ScriptableObjects/Board/New Board Section Data")]
    public class BoardSection : ScriptableObject
    {
        [Serializable]
        public enum BoardSectionCategory
        {
            Tutorial,
            SpecialTutorial,
            GeneralInformation,
            SpecialInformation,
            Quest,
            SpecialQuest
        }

        [SerializeField] BoardSectionCategory category;
        [SerializeField] int order;
        [SerializeField] string sectionTitle, description;
        [SerializeField] [TextArea] string content;
        [SerializeField] GameObject boardSectionContent;

        public int GetOrder() { return order; }
        public BoardSectionCategory GetCategory() { return category; }

        public string GetSectionTitle() { return sectionTitle; }
        public string GetDescription() { return description; }

        public string GetContent() { return content; }
        public GameObject GetBoardSectionContent() { return boardSectionContent; }

        public string GetID() { return name; }

    }

    public class BoardSectionCollection
    {
        
        private static BoardSectionCollection instance;
        public Dictionary<string, BoardSection> boardSections;

        public static BoardSectionCollection GetInstance()
        {
            if (instance == null)
            {
                instance = new BoardSectionCollection();
            }

            return instance;
        }

        BoardSectionCollection()
        {
            boardSections = new Dictionary<string, BoardSection>();
        }

        public List<BoardSection> GetBoardSections()
        {
            var rawBoardSectionList = boardSections.Values.ToList();

            rawBoardSectionList.Sort(
                (a, b) =>
                {
                    if(a.GetCategory() != b.GetCategory())
                    {
                        return a.GetCategory().CompareTo(b.GetCategory());
                    }
                    else if(a.GetOrder() != b.GetOrder())
                    {
                        return a.GetOrder().CompareTo(b.GetOrder());
                    }
                    else
                    {
                        return a.name.CompareTo(b.name);
                    }
                }
            );

            return rawBoardSectionList;
        }

        public void AddBoardSection(BoardSection boardSection)
        {
            boardSections.Add(boardSection.GetID(), boardSection);
        }

       
        public void ClearCollection()
        {
            boardSections.Clear();
        }

        
    }
}