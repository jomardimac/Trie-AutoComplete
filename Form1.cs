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

        public static string [] ArrDict => File.ReadAllLines("wordsEn.txt");

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
            public List<string> prefixes = new List<string>();
            public string prefix = null;

            public void addString(string S) {
                TrieNode n = new TrieNode();
                n = _root;
                foreach (char sc in S) {
                    n = n.AddOrGet(sc);
                }
                n = n.AddOrGet('\0');
            }

            //load the dictionary to a trei:
            public void BuildDictTrie() {
                foreach (string s in ArrDict) {
                    //build the trie by adding all strings:
                    addString(s);
                }
            }

            
            //grabs the prefixNode:
            public TrieNode PrefixNode(string s) {
                
                //Have a temporary prefix node variable and we always starting at root
                var prefNode = _root;
                //res will update everytime we go through the array
                var res = prefNode;
                foreach (var c in s) {
                    prefNode = prefNode.FindChildNode(c);
                    //if the next one is null, just end it and return the prior one
                    if (prefNode == null) {
                        break;
                    }
                    //prefix += c;
                    res = prefNode;
                }
                return res;
            }

            public void getPrefixes(string origs, TrieNode node) {
                //clear lists:
                prefixes.Clear();
                //this kills the program:
                if (string.IsNullOrEmpty(origs)) {
                    return;
                }
                foreach (var n in node.Child) {
                    BuildPrefix(origs,n.C + "", n);
                }
            }
            //grabs the string from input box and returns all the matching prefix
            public void BuildPrefix(string origs, string s, TrieNode childNode) {
                //we will be adding a suffixing string:
                //if the one has nothing in it, return;
                
                
                if (childNode.C == '\0') {
                    var fullword = origs + s;
                    prefixes.Add(fullword.Substring(0,fullword.Length-1));
                    return;
                }
                //go through each child:
                foreach (TrieNode cnode in childNode.Child) {
                    //go through each until they find a '\0; 
                    //add that character until that '\0 is found;
                        BuildPrefix(origs,s+cnode.C,cnode);
                }
            }
        }

        public void disableResultBox() {
            ResultBox.Enabled = false;
        }

        private void PrefixInputBox_TextChanged (object sender, EventArgs e) {
            
        }

        private void LoadButton_Click (object sender, EventArgs e) {
            //disable result box and clear it.
            disableResultBox();
            ResultBox.Clear();
            Root.prefixes.Clear();
            //hella slow so i created threads for functions:
            //ThreadStart getPrefs =
            //    delegate { Root.getPrefixes(PrefixInputBox.Text, Root.PrefixNode(PrefixInputBox.Text)); };
            //Thread preffixThread = new Thread(getPrefs);
            Root.getPrefixes(PrefixInputBox.Text, Root.PrefixNode(PrefixInputBox.Text));
            ThreadStart resultboxthreadstart = delegate {
                foreach (string s in Root.prefixes) {
                    ResultBox.Text += s + "\r\n";
                }
                IAsyncResult res = this.BeginInvoke(new Action(delegate { ResultBox.Enabled = true; }));
            };
            Thread resultThread = new Thread(resultboxthreadstart);
            resultThread.Start();

            //Root.getPrefixes(PrefixInputBox.Text, Root.PrefixNode(PrefixInputBox.Text));
            //foreach (string s in Root.prefixes) {
            //    ResultBox.Text += s + "\r\n";
            //}
        }
    }
}
