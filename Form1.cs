using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HW14_Dimaculangan {
    public partial class TrieForm : Form {
        public TrieForm () {
            InitializeComponent();

            Root.BuildDictTrie();
        }

        private void TrieForm_Load (object sender, EventArgs e) {
            
        }

        public static string [] ArrDict => File.ReadAllLines(@"wordsEn.txt");

        public Trie Root = new Trie();
        //now let's build the trienodes wit childs:
        public class TrieNode {
            public char C;
            public List<TrieNode> Child = new List<TrieNode>();
            
            //constructors:
            public TrieNode() {
                C = new char();
                Child = new List<TrieNode>();
            }

            public TrieNode(char newc) {
                C = newc;
               
            }

            //courtesy of evan olds: 
            public TrieNode AddOrGet(char childCharacter) {
                //go through each child and search all of it's characters. if its in none of em, its done 
                foreach (TrieNode t in Child) {
                    //
                    if (t.C == childCharacter) {
                        return t;
                    }
                }
                var n = new TrieNode(childCharacter);
                Child.Add(n);
                return n;
            }

            //find where the node is:
            public TrieNode FindChildNode(char cc) {
                foreach (var node in Child) {
                    if (node.C == cc) {
                        return node;
                    }
                }
                return null;
            }
            
        }

        //now lets build the Trie data structure:
        public class Trie {
            private readonly TrieNode _root = new TrieNode('\0');
            //public TrieNode CurNode = new TrieNode();
            public List<string> Prefixes = new List<string>();
            public string Prefix = null;

            public void AddString(string s) {
                TrieNode n = new TrieNode();
                n = _root;
                foreach (char sc in s) {
                    n = n.AddOrGet(sc);
                }
                n = n.AddOrGet('\0');
            }

            //load the dictionary to a trei:
            public void BuildDictTrie() {
                foreach (string s in ArrDict) {
                    //build the trie by adding all strings:
                    AddString(s);
                }
            }

            
            //grabs the prefixNode:
            public TrieNode PrefixNode(string s){
                
                //Have a temporary prefix node variable and we always starting at root
                var prefNode = _root;
                //res will update everytime we go through the array
                var res = prefNode;
                foreach (var c in s) {
                    prefNode = prefNode.FindChildNode(c);
                    res = prefNode;
                    //if the next one is null, just end it
                    if (prefNode == null) {
                        break;
                    }
                }
                
                return res;
            }

            public void GetPrefixes(string origs, TrieNode node) {
                //clear lists:
                Prefixes.Clear();
                if (node == null) { //no such word
                    return;
                }
                //this kills the program:
                if (string.IsNullOrEmpty(origs)) {
                    return;
                }
                foreach (var n in node.Child) {
                    BuildPrefix(origs, n.C + "", n);
                    
                }
                
            }
            //grabs the string from input box and returns all the matching prefix
            public void BuildPrefix(string origs, string s, TrieNode childNode) {
                //we will be adding a suffixing string:
                //if the one has nothing in it, return;
                if (childNode.C == '\0') {
                    var fullword = origs + s;
                    Prefixes.Add(fullword.Substring(0,fullword.Length-1));
                    return;
                }
                //go through each child:
                foreach (TrieNode cnode in childNode.Child) {
                    //go through each until they find a '\0; 
                    //add that character until that '\0 is found;
                        BuildPrefix(origs, s + cnode.C, cnode);
                }
            }
        }


        private void PrefixInputBox_TextChanged(object sender, EventArgs e) {
            //clear the prefixes and listbox items.
            Root.Prefixes.Clear();
            listBox1.Items.Clear();
            //grab the trie and everythign below it from the text change
            Root.GetPrefixes(PrefixInputBox.Text, Root.PrefixNode(PrefixInputBox.Text));
            listBox1.Items.AddRange(Root.Prefixes.ToArray());
        }
    }
}
