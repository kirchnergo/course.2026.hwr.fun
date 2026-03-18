

# Ziel 


## Programm

-   Domain Driven Design (DDD)
-   Property Based Testing
-   Exkurs: FP + Logic $\to$ [Lean4](https://lean-lang.org/)


# DDD (Domain Driven Design) 


## DDD

$\leadsto$ [Domain Driven Design](./4.1 Dmmf.pdf)

\null\hfill&#x2013;Scott Wlashin: [F# for Fun and Profit](https://fsharpforfunandprofit.com/series/designing-with-types/)


## Prinzipien

-   Verwende die Sprache der Domäne (ubiquitous Language)
-   Values und Entities
-   der Code ist das Design (kein UML nötig)
-   Design mit (algebraischen) Typen
    -   Option statt Null
    -   DU statt Vererbung
-   illegale Konstellationen sollten nicht repräsentierbar sein!


## Pause

1.  

    Are you quite sure that all those bells and whistles, all those wonderful facilities of your so called powerful programming languages, belong to the solution set rather than the problem set?
    
    \null\hfill &#x2013; Edsger Dijkstra


## DDD Übung 1 (Contacts &#x2013; ex 2)

A Contact has

-   a personal name
-   an optional email address
-   an optional postal address
-   Rule: a contact must have an email or a postal address

A Personal Name consists of a first name, middle initial, last name

-   Rule: the first name and last name are required
-   Rule: the middle initial is optional
-   Rule: the first name and last name must not be more than 50 chars
-   Rule: the middle initial is exactly 1 char, if present

A postal address consists of a four address fields plus a country

-   Rule: An Email Address can be verified or unverified


## DDD Übung 2 (Payments &#x2013; ex 3)

The payment taking system should accept:

-   Cash
-   Credit cards
-   Cheques
-   Paypal
-   Bitcoin

A payment consists of a:

-   payment
-   non-negative amount

After designing the types, create functions that will:

-   print a payment method
-   print a payment, including the amount
-   create a new payment from an amount and method


## DDD Übung 3 (Refactoring &#x2013; ex 4)

Much C# code has implicit states that you can recognize by fields called "IsSomething", or nullable date.

This is a sign that states transitions are present but not being modelled properly.


## DDD Übung 4 (Shopping Cart &#x2013; fsm ex 3)

Create types that model an e-commerce shopping cart.

-   Rule: "You can't remove an item from an empty cart"!
-   Rule: "You can't change a paid cart"!
-   Rule: "You can't pay for a cart twice"!

States are:

-   Empty
-   ActiveCartData
-   PaidCartData


## Pause

1.  

    About the use of language: it is impossible to sharpen a pencil with a blunt axe. 
    It is equally vain to try to do it with ten blunt axes instead.
    
    \null\hfill &#x2013; Edsger Dijkstra


# Property Based Testing 


## Example Based Tests :)

    module Test1 =
        open Implementation1
        let tests = testList "implementation 1" [
            test "add 1 3 = 4" {
                let actual = add 1 3
                let expected = 4
                Expect.equal expected actual "" }
            test "add 2 2 = 4" {
                let actual = add 2 2
                let expected = 4
                Expect.equal expected actual "" } ];;
    runTests expectoConfig Test1.tests

    runTests expectoConfig Test1.tests;;
    Expecto Running...
    [14:51:49 INF] EXPECTO? Running tests... <Expecto>
    [14:51:49 INF] EXPECTO! 2 tests run in 00:00:00.0366264 for implementation 1 – 2 passed, 0 ignored, 0 failed, 0 errored. Success! <Expecto>
    val it: int = 0


## Evil Developer From Hell :(

    module Implementation1 =
        let add x y =
            4

    module Implementation1 =
      val add: x: 'a -> y: 'b -> int


## PBT

$\leadsto$ [Property Based Testing](./4.2 An introduction to property based testing.pdf)

\null\hfill&#x2013;Scott Wlashin: [F# for Fun and Profit](https://fsharpforfunandprofit.com/series/property-based-testing/)


## FsCheck

    let add1 x y = x + y
    let add2 x y = x - y
    let commutativeProperty f x y =
       let result1 = f x y
       let result2 = f y x
       result1 = result2;;
    FsCheck.Check.Quick (commutativeProperty add1)
    FsCheck.Check.Quick (commutativeProperty add2)

    FsCheck.Check.Quick (commutativeProperty add1)
    FsCheck.Check.Quick (commutativeProperty add2);;
    Ok, passed 100 tests.
    Falsifiable, after 1 test (2 shrinks) (StdGen (1313711035, 297601404)):
    Original:
    1
    -1
    Shrunk:
    0
    1
    val it: unit = ()


## FsCheck (Generate)

    type Temp = F of int | C of float;;
    let fGen =
        FsCheck.Gen.choose(32,212)
        |> FsCheck.Gen.map (fun i -> F i);;
    let cGen =
        FsCheck.Gen.choose(0,100)
        |> FsCheck.Gen.map (fun i -> C (float i));;
    let tempGen =
        FsCheck.Gen.oneof [fGen; cGen]
    
    let test = tempGen |> FsCheck.Gen.sample 0 20
    test

    let tempGen =
        FsCheck.Gen.oneof [fGen; cGen]
    
    let test = tempGen |> FsCheck.Gen.sample 0 20
    test;;
    val tempGen: Gen<Temp> = Gen <fun:Bind@88>
    val test: Temp list =
      [C 5.0; C 25.0; C 15.0; F 163; F 131; F 108; F 76; C 55.0; C 45.0; C 65.0;
       C 55.0; F 124; F 92; F 69; C 75.0; C 95.0; F 80; C 93.0; C 66.0; C 11.0]
    val it: Temp list =
      [C 5.0; C 25.0; C 15.0; F 163; F 131; F 108; F 76; C 55.0; C 45.0; C 65.0;
       C 55.0; F 124; F 92; F 69; C 75.0; C 95.0; F 80; C 93.0; C 66.0; C 11.0]


## FsCheck (Shrink)

    open FsCheck
    let smallerThan81Property x = x < 81
    FsCheck.Check.Quick smallerThan81Property
    
    let test1 = FsCheck.Arb.shrink 100 |> Seq.toList
    let test2 = FsCheck.Arb.shrink 88 |> Seq.toList
    test2

    Falsifiable, after 92 tests (3 shrinks) (StdGen (1314687095, 297601404)):
    Original:
    89
    Shrunk:
    81
    val smallerThan81Property: x: int -> bool
    val test1: int list = [0; 50; 75; 88; 94; 97; 99]
    val test2: int list = [0; 44; 66; 77; 83; 86; 87]
    val it: int list = [0; 44; 66; 77; 83; 86; 87]


## Auswahl der Eigenschaften

-   Unterschiedlicher Weg, gleiches Ziel (Map(f)(Option(x))=Option(f x))
-   Hin und Her (z.B. Reverse einer Liste)
-   Invarianten (z.B. Länge einer Liste bei Sortierung)
-   Idempotenz (noch einmal bringt nichts mehr)
-   Divide et Impera! (teile und herrsche)
-   Hard to prove, easy to verify (Primzahlzerlegung)
-   Test-Orakel (z.B. einfach aber langsam)


## Pause

1.  

    Any sufficiently advanced technology is indistinguishable from magic.
    
    \null\hfill &#x2013; Arthur C. Clarke


# Exkurs: FP + Logic = Lean4 


## FP + Beweisasistent

[Lean 4](https://lean-lang.org/) ist sowohl eine funktionale Programmiersprache (ähnlich wie F#) als auch ein interaktiver Theorembeweiser.

Du kannst damit also sowohl eine App schreiben als auch mathematisch lückenlos beweisen, dass sie genau das tut, was sie soll.

1.  Curry-Howard-Isomorphismus

    In Lean sind *Sätze* (Theoreme) ****Typen**** und *Beweise* sind ****Programme****. 
    
    Wenn du ein Programm schreibst, das einen bestimmten Typ zurückgibt, 
    hast du gleichzeitig den Beweis für die entsprechende mathematische Aussage geliefert.


## Eigenschaften I

-   General-Purpose-Power
    -   Lean 4 eine vollwertige Allzweck-Programmiersprache. Sie ist fast komplett in Lean selbst geschrieben ("self-hosted"), was sie extrem flexibel und performant macht.
-   Kompilation nach C
    -   Lean 4 kompiliert über einen Zwischenschritt direkt nach C. Das bedeutet, dass der Code sehr effizient ist. Man kann Lean also theoretisch für Systemprogrammierung nutzen, bei der absolute Korrektheit überlebenswichtig ist.


## Eigenschaften II

-   Metaprogrammierung
    -   Eines der mächtigsten Features: Du kannst die Sprache erweitern. Wenn dir die Syntax nicht gefällt oder du eine eigene "Domain Specific Language" (DSL) brauchst, kannst du Lean so umprogrammieren, dass es deine Befehle versteht.
-   Taktiken (Tactics)
    -   Anstatt Beweise mühsam händisch Schritt für Schritt zu tippen, nutzt man in Lean oft Taktiken. Das sind kleine Automatismen, die dem System sagen: "Löse diesen Teil durch Induktion" oder "Vereinfache diese Gleichung". Das macht das Beweisen interaktiv und fast wie ein Puzzle.


## Einführung

$\leadsto$ [Lean Game Server](https://adam.math.hhu.de/#/)


# Ende 


## Zusammenfassung

-   funktionales Domain Modeling (DDD)
-   eigenschaftsbasiertes Testen (Property Based Testing)


## Links

-   [Domain Driven Design](https://fsharpforfunandprofit.com/ddd/)
-   [Domain Modeling Made Functional](https://fsharpforfunandprofit.com/books/)
-   [FsCheck](https://github.com/fscheck/FsCheck)
-   [An introduction to property-based testing](https://fsharpforfunandprofit.com/posts/property-based-testing/)
-   [Choosing properties for property-based testing](https://fsharpforfunandprofit.com/posts/property-based-testing-2/)


## Hausaufgabe  (Erinnerung)

-   exercism.io (bis 23.03)
    -   [ ] Poker (Programmieraufgabe)


## Termine

-   [X] 18.02 13:00 - 17:15
-   [X] 25.02 13:00 - 17:15
-   [X] 04.03 13:00 - 17:15
-   [X] 18.03 13:00 - 17:15
-   [ ] 25.03 13:00 - 17:15

