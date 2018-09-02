namespace RootFinder

open System
open Complex
open Utilities

[<NoComparison>]
[<CustomEquality>]
type Polynomial =
  struct
    val coefficients: Complex array
    new (coefficients) = { coefficients = coefficients}
  end

  interface IEquatable<Polynomial> with
    member p.Equals q =
      sequence_equal p.coefficients q.coefficients

  override p.ToString() = "[ " + String.Join(", " , p.coefficients :> seq<_>) + " ]"

  member inline p.degree =
    p.coefficients.Length - 1

  member inline p.Item(index) =
    p.coefficients.[index]

  member p.derivative =
    let degree = p.degree
    let array = Array.zeroCreate degree
    for i = 0 to degree - 1 do
      array.[i] <- (i + 1) * p.[i + 1]
    Polynomial array

  // Horner's Rule
  member p.eval(z: Complex) =
    let mutable total = Complex.zero
    for i = p.degree downto 0 do
      total <- p.[i] + z * total
    total

  static member (*) (s: Complex, p: Polynomial) =
    Polynomial <| Array.map (fun z -> s * z) p.coefficients

  static member (*) (s: double, p: Polynomial) =
    Polynomial <| Array.map (fun z -> s * z) p.coefficients

  static member (/) (p: Polynomial, s: double) =
    Polynomial <| Array.map (fun z -> z / s) p.coefficients

  static member (+) (p1: Polynomial, p2: Polynomial) =
    let max = Math.Max(p1.degree, p2.degree)
    let min = Math.Min(p1.degree, p2.degree)
    let array = Array.zeroCreate (max + 1)
    for i = 0 to min do
      array.[i] <- p1.[i] + p2.[i]
    Polynomial array

  static member (-) (p1: Polynomial, p2: Polynomial) =
      let max = Math.Max(p1.degree, p2.degree)
      let min = Math.Min(p1.degree, p2.degree)
      let array = Array.zeroCreate (max + 1)
      for i = 0 to min do
        array.[i] <- p1.[i] - p2.[i]
      Polynomial array

  static member (*) (p: Polynomial, q: Polynomial) =
    let array = Array.zeroCreate (p.degree + q.degree + 1)
    for i = 0 to array.Length - 1 do
      for j = Math.Min (i, p.degree) downto Math.Max (0, i - q.degree) do
        array.[i] <- array.[i] + p.[j] * q.[i - j]
    Polynomial array

module Polynomial =
  let inline poly seq =
    Seq.map complex seq |> Seq.toArray |> Polynomial
