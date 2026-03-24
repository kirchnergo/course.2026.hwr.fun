

# Ziel 


## Programm

-   Hausaufgaben
-   Domain Driven Design (DDD)
-   Property Based Testing
-   Exkurs: FP + Logic $\to$ [Lean4](https://lean-lang.org/)


# Hausaufgaben 


## Accumulate

    let rec accumulateR func input acc = 
        match input with
        | [] -> acc |> List.rev
        | head::tail -> accumulateR func tail (func head :: acc)
    let accumulate func input = accumulateR func input []
    let test1 = accumulate (fun x -> x * x) [1; 2; 3]
    let test2 = accumulate (fun (x:string) -> x.ToUpper()) ["hello"; "world"]

    val accumulateR: func: ('a -> 'b) -> input: 'a list -> acc: 'b list -> 'b list
    val accumulate: func: ('a -> 'b) -> input: 'a list -> 'b list
    val test1: int list = [1; 4; 9]
    val test2: string list = ["HELLO"; "WORLD"]


## Space Age

    type Planet = 
        | Mercury
        | Venus
        | Earth
        | Mars
        | Jupiter
        | Saturn
        | Uranus
        | Neptune
    let orbitalPeriodRelativeToEarthOn planet = 
        match planet with
        | Mercury -> 0.2408467
        | Venus -> 0.61519726
        | Earth -> 1.0
        | Mars -> 1.8808158
        | Jupiter -> 11.862615
        | Saturn -> 29.447498
        | Uranus -> 84.016846
        | Neptune -> 164.79132


## Space Age (II)

    open System
    [<Literal>]
    let SecondsInOneEarthYear = 31557600.0
    let secondsInAYearOn planet =
        SecondsInOneEarthYear * orbitalPeriodRelativeToEarthOn planet
    let round (number : float) = Math.Round(number, 2)
    let age (planet: Planet) (seconds: int64): float =
        float seconds / (secondsInAYearOn planet)
        |> round
    let test1 = age Earth 1000000000L

    [<Literal>]
    val SecondsInOneEarthYear: float = 31557600
    val secondsInAYearOn: planet: Planet -> float
    val round: number: float -> float
    val age: planet: Planet -> seconds: int64 -> float
    val test1: float = 31.69


## Zusammenfassung

-   nutze [exercism.io](https://exercism.io)!
-   Vermeide `mutable`!!
-   nur wichtiges verdient einen Namen
-   Vertraue der **Pipe** (`>>`, `|>`, &#x2026;)!!
-   If-Then-Else mit Boolean ist unnötig
-   Parametrisiere!
-   If-Then-Else vermeiden &#x2026; besser `match`!
-   Be lazy! (vermeide `for`-loops)
-   [Troubleshooting F#](https://fsharpforfunandprofit.com/troubleshooting-fsharp/)
-   [F#-Styleguide](https://docs.microsoft.com/de-de/dotnet/fsharp/style-guide/)


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
    
      FsCheck.Check.Quick (commutativeProperty add1)
      ^^^^^^^
    
    /Users/kirchnerg/Desktop/courses/course.2026.hwr.fun/slides/stdin(309,1): error FS0039: The value, namespace, type or module 'FsCheck' is not defined.


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
    
          FsCheck.Gen.oneof [fGen; cGen]
      ----^^^^^^^
    
    /Users/kirchnerg/Desktop/courses/course.2026.hwr.fun/slides/stdin(320,5): error FS0039: The value, namespace, type or module 'FsCheck' is not defined.


## FsCheck (Shrink)

    open FsCheck
    let smallerThan81Property x = x < 81
    FsCheck.Check.Quick smallerThan81Property
    
    let test1 = FsCheck.Arb.shrink 100 |> Seq.toList
    let test2 = FsCheck.Arb.shrink 88 |> Seq.toList
    test2

    
      open FsCheck
      -----^^^^^^^
    
    /Users/kirchnerg/Desktop/courses/course.2026.hwr.fun/slides/stdin(325,6): error FS0039: The namespace or module 'FsCheck' is not defined.


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

ist sowohl eine funktionale Programmiersprache (ähnlich wie F#) als auch ein interaktiver Theorembeweiser.

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

