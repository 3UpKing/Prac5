  // Do learn to insert your names and a brief description of
  // what the program is supposed to do!

  // This is a skeleton program for developing a parser for C declarations
  // Chifamba, Chiyoka, Mutowo, Ngwarai

  using Library;
  using System;
  using System.Text;

  class Token {
    public int kind;
    public string val;

    public Token(int kind, string val) {
      this.kind = kind;
      this.val = val;
    } // constructor

  } // Token

  class Declarations {

    // +++++++++++++++++++++++++ File Handling and Error handlers ++++++++++++++++++++

    static InFile input;
    static OutFile output;

    static string NewFileName(string oldFileName, string ext) {
    // Creates new file name by changing extension of oldFileName to ext
      int i = oldFileName.LastIndexOf('.');
      if (i < 0) return oldFileName + ext; else return oldFileName.Substring(0, i) + ext;
    } // NewFileName

    static void ReportError(string errorMessage) {
    // Displays errorMessage on standard output and on reflected output
      Console.WriteLine(errorMessage);
      output.WriteLine(errorMessage);
    } // ReportError

    static void Abort(string errorMessage) {
    // Abandons parsing after issuing error message
      ReportError(errorMessage);
      output.Close();
      System.Environment.Exit(1);
    } // Abort

    // +++++++++++++++++++++++  token kinds enumeration +++++++++++++++++++++++++

    const int
      noSym         =   0,
      EOFSym        =   1,
      identSym      =   2,
      numSym        =   3,
      lssSym        =   4,
      leqSym        =   5,
      geqSym        =   6,
      eqlSym        =   7,  
      neqSym        =   8,
      orSym         =   9,
      andSym        =   10,
      gtrSym        =   11,
      assignSym     =   12,
      notSym        =   13,
      LeftRndBracSym  =   14,
      RightRndBracSym =   15,
      commaSym      =   16,
      starSym       =   17,
      dotSym        =   18,
      semiColonSym  =   19,
      LeftSquareBracSym = 20,
      RightSquareBracSym = 21,
      BackSlashSym  =   22,
      DashSym       =   23,
      plusSym       =   24
      ;

    // +++++++++++++++++++++++++++++ Character Handler ++++++++++++++++++++++++++

    const char EOF = '\0';
    static bool atEndOfFile = false;

    // Declaring ch as a global variable is done for expediency - global variables
    // are not always a good thing

    static char ch;    // look ahead character for scanner

    static void GetChar() {
    // Obtains next character ch from input, or CHR(0) if EOF reached
    // Reflect ch to output
      if (atEndOfFile) ch = EOF;
      else {
        ch = input.ReadChar();
        atEndOfFile = ch == EOF;
        if (!atEndOfFile) output.Write(ch);
      }
    } // GetChar

    // +++++++++++++++++++++++++++++++ Scanner ++++++++++++++++++++++++++++++++++

    // Declaring sym as a global variable is done for expediency - global variables
    // are not always a good thing

    static Token sym;

    static void GetSym() {
    // Scans for next sym from input
      while (ch > EOF && ch <= ' ') GetChar();
      StringBuilder symLex = new StringBuilder();      

      int symKind = noSym;
        if (Char.IsLetter(ch)) {
        do {
        symLex.Append(ch); GetChar();
        } 
        while (Char.IsLetterOrDigit(ch));
        symKind = identSym;
        }
        else if (Char.IsDigit(ch)) {
        do {
        symLex.Append(ch); GetChar();
        } while (Char.IsDigit(ch));
        symKind = numSym;
        }
        else {
        symLex.Append(ch);


      switch (ch) {
        case EOF:
        symLex = new StringBuilder("EOF"); // special case
        symKind = EOFSym; break; // no need to GetChar
        case '<':
        symKind = lssSym; GetChar();

        if (ch == '=') {
        symLex.Append(ch); symKind = leqSym; GetChar();
        }
        break;

        case '>':
        //symKind = gtrSym; GetChar();
        if (ch == '=') {
        symLex.Append(ch); symKind = geqSym; GetChar();
        }
        break;
        case '=':
        //symKind = assignSym; GetChar();
        if (ch == '=') {
        symLex.Append(ch); symKind = eqlSym; GetChar();
        }
        break;
        case '!':
        //symKind = notSym; GetChar();
        if (ch == '=') {
        symLex.Append(ch); symKind = neqSym; GetChar();
        }
        break;
        case '|':
        //symKind = noSym; GetChar();
        if (ch == '|') {
        symLex.Append(ch); symKind = orSym; GetChar();
        }
        break;
        case '&':
        //symKind = noSym; GetChar();
        if (ch == '&') {
        symLex.Append(ch); symKind = andSym; GetChar();
        }
        break;
        case '(':
         //symKind = noSym; GetChar();
        if (ch == '(') {
        symLex.Append(ch); symKind = LeftRndBracSym; GetChar();
        }
        break;
         case ')':
         //symKind = noSym; GetChar();
        if (ch == ')') {
        symLex.Append(ch); symKind = RightRndBracSym; GetChar();
        }
        break;
         case ',':
         //symKind = noSym; GetChar();
        if (ch == ',') {
        symLex.Append(ch); symKind = commaSym; GetChar();
        }
        break;

         case '-':
         //symKind = noSym; GetChar();
        if (ch == '-') {
        symLex.Append(ch); symKind = DashSym; GetChar();
        }
        break;
         case '+':
        // symKind = noSym; GetChar();
        if (ch == '+') {
        symLex.Append(ch); symKind = plusSym; GetChar();
        }
        break;

        default:        
          symKind = noSym; GetChar();
        break;
      }
      // over to you!

      sym = new Token(symKind, symLex.ToString());
    }
    } // GetSym

  /*  ++++ Commented out for the moment

    // +++++++++++++++++++++++++++++++ Parser +++++++++++++++++++++++++++++++++++

    static void Accept(int wantedSym, string errorMessage) {
    // Checks that lookahead token is wantedSym
      if (sym.kind == wantedSym) GetSym(); else Abort(errorMessage);
    } // Accept

    static void Accept(IntSet allowedSet, string errorMessage) {
    // Checks that lookahead token is in allowedSet
      if (allowedSet.Contains(sym.kind)) GetSym(); else Abort(errorMessage);
    } // Accept

    static void CDecls() {}

  ++++++ */

    // +++++++++++++++++++++ Main driver function +++++++++++++++++++++++++++++++

    public static void Main(string[] args) {
      // Open input and output files from command line arguments
      if (args.Length == 0) {
        Console.WriteLine("Usage: Declarations FileName");
        System.Environment.Exit(1);
      }
      input = new InFile(args[0]);
      output = new OutFile(NewFileName(args[0], ".out"));

      GetChar();                                  // Lookahead character

  //  To test the scanner we can use a loop like the following:
  
      do {
        GetSym();                                 // Lookahead symbol
        OutFile.StdOut.Write(sym.kind, 3);
        OutFile.StdOut.WriteLine(" " + sym.val);  // See what we got
      } while (sym.kind != EOFSym);

  /*  After the scanner is debugged we shall substitute this code:

      GetSym();                                   // Lookahead symbol
      CDecls();                                   // Start to parse from the goal symbol
      // if we get back here everything must have been satisfactory
      Console.WriteLine("Parsed correctly");

  */
      output.Close();
    } // Main

  } // Declarations

