# Yksinkertainen "tekstieditori"

Simppeli C#-sovellus, joka demonstroi **Memento-suunnittelumallin** toimintaa undo- ja redo-toimintojen avulla.

## Kuvaus

Hyvin yksinkertainen C#-sovellus, jonka tarkoituksena on havainnollistaa **Memento-suunnittelumallin** toimintaa.
Sovellus toimii hyvin pelkistettynä tekstieditorina, jossa käyttäjä voi kirjoittaa tekstiä rivi kerrallaan sekä käyttää **undo**- ja **redo**-toimintoja.

## Toiminnot

Ohjelmassa on kolme päätoimintoa:

* `Kirjoita` – lisää uuden tekstirivin dokumenttiin
* `Undo` – palauttaa edellisen dokumenttiversion
* `Redo` – palauttaa seuraavan dokumenttiversion

Aina ennen kuin rivien määrä muuttuu joko kirjoittamalla tai **undo**- ja **redo**-toiminnoilla, dokumentista tallennetaan uusi **memento**, joka sisältää dokumentin tilan sillä hetkellä.

## Dokumentin rakenne

Dokumentilla on kaksi ominaisuutta:

* `Teksti` – dokumentin teksti
* `Versio` – dokumentin versionumero

Dokumentin versionumero kasvaa tekstirivin lisäyksen yhteydessä (ja laskee Undo-toiminnolla).

## Tarkoitus

Tehty ainoastaan kouluprojektiin ja Memento-suunnittelumallin esittelyyn.
Sovellus ei sisällä "oikean" tekstieditorin ominaisuuksia. Tiedostoon tallennusta ei ole, vaan dokumentin tiloja säilytetään ainoastaan väliaikaisesti **mementoissa**.

## Teknologiat

* C#
* .NET-konsolisovellus

