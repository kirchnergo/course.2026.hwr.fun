module Poker

// TODO: implement this module

// Define basic card-game elements
type Suit = 
  | Clubs
  | Diamonds
  | Hearts
  | Spades

type Number = 
  | Two
  | Three
  | Four
  | Five
  | Six
  | Seven
  | Eight
  | Nine
  | Ten
  | Ace
  | Jack
  | Queen
  | King

// Define compositions
type Card = Number * Suit
type Hand = Card array


let matchSuit(suit: string): Suit = 
  match suit with
  | "C" -> Clubs 
  | "D" -> Diamonds
  | "H" -> Hearts
  | "S" -> Spades
  | _   -> failwith "Suit does not exist"

let matchNumber(number: string): Number = 
  match number with
  | "2"  -> Two
  | "3"  -> Three
  | "4"  -> Four
  | "5"  -> Five
  | "6"  -> Six
  | "7"  -> Seven
  | "8"  -> Eight
  | "9"  -> Nine
  | "10" -> Ten
  | "A"  -> Ace
  | "J"  -> Jack
  | "Q"  -> Queen
  | "K"  -> King
  | _    -> failwith "Number does not exist"

let cardParser (cardString: string): Card = 
  let len = cardString.Length
  let numStr, suitStr =
      if len = 3 then cardString.[0..1], cardString.[2..2]
      else cardString.[0..0], cardString.[1..1]
  matchNumber numStr, matchSuit suitStr

let handParser (handString: string): Hand = 
  handString.Split ' ' |> Array.map cardParser

let cardToString ((n, s): Card) =
  let numStr =
    match n with
    | Two -> "2"
    | Three -> "3"
    | Four -> "4"
    | Five -> "5"
    | Six -> "6"
    | Seven -> "7"
    | Eight -> "8"
    | Nine -> "9"
    | Ten -> "10"
    | Jack -> "J"
    | Queen -> "Q"
    | King -> "K"
    | Ace -> "A"
  let suitStr =
    match s with
    | Clubs -> "C"
    | Diamonds -> "D"
    | Hearts -> "H"
    | Spades -> "S"
  numStr + suitStr

let cardValue (n: Number): int =
  match n with
  | Two -> 2
  | Three -> 3
  | Four -> 4
  | Five -> 5
  | Six -> 6
  | Seven -> 7
  | Eight -> 8
  | Nine -> 9
  | Ten -> 10
  | Jack -> 11
  | Queen -> 12
  | King -> 13
  | Ace -> 14

// Determine the rank of a hand.
// Returns a tuple: (category, tieBreakers)
// Categories (in increasing order):
//   0 = High card
//   1 = One pair
//   2 = Two pairs
//   3 = Three of a kind
//   4 = Straight
//   5 = Flush
//   6 = Full house
//   7 = Four of a kind
//   8 = Straight flush
let rankHand (hand: Hand) : int * int list =
  let values = hand |> Array.map (fun (n, _) -> cardValue n)
  let sortedValuesDesc = values |> Array.sortDescending |> Array.toList
  let isFlush = hand |> Array.map (fun (_, s) -> s) |> Array.distinct |> Array.length = 1

  let sortedValuesAsc = values |> Array.sort |> Array.toList
  // Check "aufeinanderfolgende" values
  let isConsecutive lst =
      lst |> List.pairwise |> List.forall (fun (a, b) -> b = a + 1)
  // Determine if straight (including the case A 2 3 4 5)
  let straightHigh =
      if isConsecutive sortedValuesAsc then List.last sortedValuesAsc
      elif sortedValuesAsc = [2;3;4;5;14] then 5
      else -1
  let isStraight = straightHigh <> -1

  let groups = 
       sortedValuesDesc
       |> List.groupBy id
       |> List.map (fun (v, lst) -> v, List.length lst)
       |> List.sortBy (fun (v, count) -> -count, -v)

  let category, tiebreakers =
    if isStraight && isFlush then
      8, [straightHigh]
    elif groups |> List.exists (fun (_, count) -> count = 4) then
      let quad = groups |> List.find (fun (_, count) -> count = 4) |> fst
      let kicker = groups |> List.find (fun (_, count) -> count <> 4) |> fst
      7, [quad; kicker]
    elif groups.Head |> snd = 3 && groups.Tail.Head |> snd = 2 then
      // Full house
      let trip = groups.Head |> fst
      let pair = groups.Tail.Head |> fst
      6, [trip; pair]
    elif isFlush then
      // Flush
      5, sortedValuesDesc
    elif isStraight then
      // Straight
      4, [straightHigh]
    elif groups.Head |> snd = 3 then
      // Three of a kind
      let trip = groups.Head |> fst
      let kickers = groups |> List.filter (fun (_, count) -> count = 1) |> List.map fst |> List.sortDescending
      3, trip :: kickers
    elif groups.Head |> snd = 2 && (groups.Tail.Head |> snd = 2) then
      // Two pairs
      let pair1 = groups.Head |> fst
      let pair2 = groups.Tail.Head |> fst
      let kicker = groups |> List.filter (fun (_, count) -> count = 1) |> List.head |> fst
      2, [pair1; pair2; kicker]
    elif groups.Head |> snd = 2 then
      // One pair
      let pair = groups.Head |> fst
      let kickers = groups |> List.filter (fun (_, count) -> count = 1) |> List.map fst |> List.sortDescending
      1, pair :: kickers
    else
      // High card
      0, sortedValuesDesc
  category, tiebreakers

// Gib beste Karten zurück
let getBestHands (hands: Hand array): Hand array =
  let ranked = hands |> Array.map (fun hand -> (hand, rankHand hand))
  let bestRank = ranked |> Array.map snd |> Array.max
  ranked |> Array.filter (fun (_, r) -> r = bestRank) |> Array.map fst

// returns the best hands as strings
let bestHands (hands: string list): string list =
  hands 
  |> List.map handParser 
  |> Array.ofList 
  |> getBestHands
  |> Array.map (fun hand -> hand |> Array.map cardToString |> String.concat " ")
  |> Array.toList