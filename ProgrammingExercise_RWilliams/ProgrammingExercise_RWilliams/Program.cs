/*
 Author: Rachel Williams 
 Description: Code for the programming exercise asked to parse through data from JSON files
             to obtain and display search results from user input
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace ProgrammingExercise_RWilliams
{
    class Program
    {
        static Dictionary<int, string[]> OrganizeData(Dictionary<int, string[]> dictionary, List<object> listObj)
        {
            int i = 0;
            int arrayLength = 0;

            // Setting appropriate array length for contacts and outlets list 
            if (listObj.Count < 100) arrayLength = 2;
            else arrayLength = 6; 

            for (int k = 0; k < listObj.Count(); k++)
            {
                string check = listObj.ElementAt(k).ToString();
                if (check.Contains("id\":"))
                {
                    string[] infoArray = new string[arrayLength];
                    for (int j = 0; j < infoArray.Length; j++) 
                    { 
                        // Trimming extra characters 
                        string temp = listObj.ElementAt(k).ToString();
                        temp = temp.Substring(temp.IndexOf(":")+1);
                        temp = temp.Substring(temp.IndexOf("\"") + 1);
                        char[] trimChars = { ',', '\"'  };
                        temp = temp.TrimEnd(trimChars);
                        temp = temp.TrimStart('\"').TrimEnd('\"');
                        infoArray[j] = temp; 
                        k++;
                    }     
                    dictionary.Add(i, infoArray); i++;
                }
            }
            return dictionary;
        }

        static void SearchData(string[] keyWords)
        {
            Dictionary<int, string[]> allContacts = new Dictionary<int, string[]>();
            Dictionary<int, string[]> allOutlets = new Dictionary<int, string[]>();
            List<object> contactsInfo = new List<object>();
            List<object> outletsInfo = new List<object>();
            Dictionary<int, object> searchResults = new Dictionary<int, object>();
            
            // Read JSON file data
            StreamReader contactsFile = File.OpenText("Contacts.json");
            StreamReader outletsFile = File.OpenText("Outlets.json");
            string contactsFileData; 
            string outletsFileData; 

            // Put data into List 
            while ((contactsFileData = contactsFile.ReadLine()) != null) contactsInfo.Add(contactsFileData);
            while ((outletsFileData = outletsFile.ReadLine()) != null) outletsInfo.Add(outletsFileData);

            allContacts = OrganizeData(allContacts,contactsInfo);
            allOutlets = OrganizeData(allOutlets, outletsInfo);

            // Search for keywords in contacts data 
            int index = 0;
            int contactsIndex = 0;
            int checkIndex = 0;
            var values = allContacts.Values;

            for (int i=0; i < keyWords.Length; i++ )
            {
                foreach (var item in values.ToList()) 
                {
                    foreach (var subItem in item) 
                    {
                        if (!(keyWords[i].Equals("")))
                        {
                            if (subItem.Contains(keyWords[i]))
                            {
                                if (contactsIndex < allContacts.Count) 
                                {
                                    // Remove from master list, update list and indexes 
                                    var getKey = allContacts.ElementAt(contactsIndex);
                                    var keyToRemove = getKey.Key;
                                    allContacts.Remove(keyToRemove);
                                    if (index.Equals(checkIndex)) index++;
                                    searchResults.Add(index, item);
                                    checkIndex = index;
                                }
                            } 
                        }   
                    }
                    index++; contactsIndex++; 
                }
                contactsIndex = 0;
            }

            // Get outlet name from outletid in outlets data 
            int outletIndex = 0;
            for (int i = 0; i < searchResults.Count; i++)
            {
                string[] outletID = (String[])searchResults.ElementAt(i).Value;
                string id = outletID[1];

                foreach (var item in allOutlets.Values)
                {
                    foreach (string subItem in item)
                    {
                        if (subItem.Contains(id)) 
                        {
                            var outletKey = searchResults.ElementAt(i).Key; 
                            outletID[1] = item[1];
                            searchResults.Remove(outletKey);
                            searchResults.Add(outletKey, outletID);
                        } 
                    }
                    outletIndex++;
                }
            }
             
            // Display info to console 
            Console.WriteLine("Below are the search results from your keywords:");
            Console.WriteLine(); 
            for (int i = 0; i < searchResults.Count; i++)
            {
                string[] finalResult = (String[])searchResults.ElementAt(i).Value;
                string temp = finalResult[2];
                Console.WriteLine(finalResult[2] + " " + finalResult[3]);
                Console.WriteLine(finalResult[4]);
                Console.WriteLine(finalResult[1]);
                Console.WriteLine(finalResult[5]);
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            // Prompt user for search terms, read in user response 
            Console.WriteLine("Please enter 1 or more keywords separated by a space: ");
            string userResponse = Console.ReadLine();
            string[] keyWords = userResponse.Split(' ');
            Console.WriteLine();

            // Check if key word were entered, search JSON file for keywords 
            int counter=0; 
            foreach (var item in keyWords) { 
                var result = item.Equals("");
                if (result) counter++; 
            }
            if (counter.Equals(keyWords.Length))Console.WriteLine("No keywords were given. Please restart");
            else SearchData(keyWords);
        }
    }
}
