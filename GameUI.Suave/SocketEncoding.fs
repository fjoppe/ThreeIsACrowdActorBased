namespace GameUI.Suave.ThreeIsACrowd

//  borrowed from: https://github.com/SuaveIO/suave/blob/master/paket-files/haf/YoLo/YoLo.fs

open System

type Base64String = string

module String =
  open System.IO
  open System.Security.Cryptography

  /// Also, invariant culture
  let equals (a : string) (b : string) =
    a.Equals(b, StringComparison.InvariantCulture)

  /// Also, invariant culture
  let equalsCaseInsensitve (a : string) (b : string) =
    a.Equals(b, StringComparison.InvariantCultureIgnoreCase)
    
  /// Compare ordinally with ignore case.
  let equalsOrdinalCI (str1 : string) (str2 : string) =
    String.Equals(str1, str2, StringComparison.OrdinalIgnoreCase)

  /// Ordinally compare two strings in constant time, bounded by the length of the
  /// longest string.
  let equalsConstantTime (str1 : string) (str2 : string) =
    let mutable xx = uint32 str1.Length ^^^ uint32 str2.Length
    let mutable i = 0
    while i < str1.Length && i < str2.Length do
      xx <- xx ||| uint32 (int str1.[i] ^^^ int str2.[i])
      i <- i + 1
    xx = 0u

  let toLowerInvariant (str : string) =
    str.ToLowerInvariant()

  let replace (find : string) (replacement : string) (str : string) =
    str.Replace(find, replacement)

  let isEmpty (s : string) =
    s.Length = 0

  let trim (s : string) =
    s.Trim()
  
  let trimc (toTrim : char) (s : string) =
    s.Trim toTrim
  
  let trimStart (s : string) =
    s.TrimStart()
  
  let split (c : char) (s : string) =
    s.Split c |> Array.toList
  
  let splita (c : char) (s : string) =
    s.Split c
  
  let startsWith (substring : string) (s : string) =
    s.StartsWith substring
  
  let contains (substring : string) (s : string) =
    s.Contains substring
  
  let substring index (s : string) =
    s.Substring index

module Bytes =
  open System.IO
  open System.Linq
  open System.Security.Cryptography

  let hash (algo : HashAlgorithm) (bs : byte[]) =
    use ms = new MemoryStream()
    ms.Write(bs, 0, bs.Length)
    ms.Seek(0L, SeekOrigin.Begin) |> ignore
    use sha = algo
    sha.ComputeHash ms

  let sha1 =
    hash (new SHA1Managed())

  let sha256 =
    hash (new SHA256Managed())

  let sha512 =
    hash (new SHA512Managed())

  let toHex (bs : byte[]) =
    BitConverter.ToString bs
    |> String.replace "-" ""
    |> String.toLowerInvariant

  let fromHex (digestString : string) =
    Enumerable.Range(0, digestString.Length)
              .Where(fun x -> x % 2 = 0)
              .Select(fun x -> Convert.ToByte(digestString.Substring(x, 2), 16))
              .ToArray()

  /// Compare two byte arrays in constant time, bounded by the length of the
  /// longest byte array.
  let equalsConstantTime (bits : byte []) (bobs : byte []) =
    let mutable xx = uint32 bits.Length ^^^ uint32 bobs.Length
    let mutable i = 0
    while i < bits.Length && i < bobs.Length do
      xx <- xx ||| uint32 (bits.[i] ^^^ bobs.[i])
      i <- i + 1
    xx = 0u


module UTF8 =
  open System.Text

  let private utf8 = Encoding.UTF8

  /// Convert the full buffer `b` filled with UTF8-encoded strings into a CLR
  /// string.
  let toString (bs : byte []) =
    utf8.GetString bs

  /// Convert the byte array to a string, by indexing into the passed buffer `b`
  /// and taking `count` bytes from it.
  let toStringAtOffset (b : byte []) (index : int) (count : int) =
    utf8.GetString(b, index, count)

  /// Get the UTF8-encoding of the string.
  let bytes (s : string) =
    utf8.GetBytes s

  /// Convert the passed string `s` to UTF8 and then encode the buffer with
  /// base64.
  let encodeBase64 : string -> Base64String =
    bytes >> Convert.ToBase64String

  /// Convert the passed string `s`, assumed to be a valid Base64 encoding, to a
  /// CLR string, going through UTF8.
  let decodeBase64 : Base64String -> string =
    Convert.FromBase64String >> toString

  let sha1 =
    bytes >> Bytes.sha1

  let sha1Hex =
    bytes >> Bytes.sha1 >> Bytes.toHex

  let sha256 =
    bytes >> Bytes.sha256

  let sha256Hex =
    bytes >> Bytes.sha256 >> Bytes.toHex

  let sha512 =
    bytes >> Bytes.sha512

  let sha512Hex =
    bytes >> Bytes.sha512 >> Bytes.toHex