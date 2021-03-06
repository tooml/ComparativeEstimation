﻿using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Gewichtung.Model;
using Shouldly;
using Xunit;

namespace Gewichtung.Tests
{
    public class GewichtungTests
    {

        [Fact]
        public void Gewichtung_berechnen()
        {
            var voting = new List<GewichtetesVergleichspaarDto>()
            {
                new GewichtetesVergleichspaarDto {Id = "1", Selektion = Selektion.A},
                new GewichtetesVergleichspaarDto {Id = "2", Selektion = Selektion.B},
                new GewichtetesVergleichspaarDto {Id = "3", Selektion = Selektion.B},
            };

            var paare = new List<Vergleichspaar>()
            {
                new Vergleichspaar {Id = "1", IndexA = 0, IndexB = 1},
                new Vergleichspaar {Id = "2", IndexA = 0, IndexB = 2},
                new Vergleichspaar {Id = "3", IndexA = 1, IndexB = 2}
            };

            var sut = new global::Gewichtung.Gewichtung();
            sut.Gewichtung_berechne(
                voting,
                paare,
                g => g.StoryIndizes.ShouldBe(new[] { 2, 0, 1 }),
                () => { throw new Exception(); }
            );
        }

        [Fact]
        public void GesamtgewichtungBerechnen_nochKeineVotings_leereListe()
        {
            var sut = new global::Gewichtung.Gewichtung();

            var result = sut.Gesamtgewichtung_berechne(new List<Contracts.Gewichtung>());
            result.StoryIndizes.ShouldBeEmpty();

            result = sut.Gesamtgewichtung_berechne(null);
            result.StoryIndizes.ShouldBeEmpty();
        }

        [Fact]
        public void Gewichtung_berechnen_FehlerTrittAuf()
        {
            var voting = new List<GewichtetesVergleichspaarDto>()
            {
                new GewichtetesVergleichspaarDto {Id = "1", Selektion = Selektion.A},
                new GewichtetesVergleichspaarDto {Id = "2", Selektion = Selektion.B},
                new GewichtetesVergleichspaarDto {Id = "3", Selektion = Selektion.A},
            };

            var paare = new List<Vergleichspaar>()
            {
                new Vergleichspaar {Id = "1", IndexA = 0, IndexB = 1},
                new Vergleichspaar {Id = "2", IndexA = 0, IndexB = 2},
                new Vergleichspaar {Id = "3", IndexA = 1, IndexB = 2}
            };

            var sut = new global::Gewichtung.Gewichtung();
            sut.Gewichtung_berechne(
                voting,
                paare,
                g => { throw new Exception("kein Fehler"); },
                () => Assert.True(true)
            );
        }


        [Fact]
        public void Relationen()
        {
            IEnumerable<GewichtetesVergleichspaarDto> voting = new List<GewichtetesVergleichspaarDto>
                {
                    new GewichtetesVergleichspaarDto
                    {
                        Id = "0",
                        Selektion = Selektion.A
                    },
                    new GewichtetesVergleichspaarDto
                    {
                        Id = "1",
                        Selektion = Selektion.B
                    },
                    new GewichtetesVergleichspaarDto
                    {
                        Id = "2",
                        Selektion = Selektion.B
                    },
                };

            IEnumerable<Vergleichspaar> vergleichspaare = new List<Vergleichspaar>
            {
                new Vergleichspaar
                {
                    Id = "0",
                    IndexA = 0,
                    IndexB = 1,
                },
                new Vergleichspaar
                {
                    Id = "1",
                    IndexA = 0,
                    IndexB = 2,
                },
                new Vergleichspaar
                {
                    Id = "2",
                    IndexA = 1,
                    IndexB = 2,
                },
            };
            var result = Gewichtung.Relationen_erstellen(voting, vergleichspaare).ToList();
            AssertIndize(result[0], 0, 1);
            AssertIndize(result[1], 2, 0);
            AssertIndize(result[2], 2, 1);
        }

        private void AssertIndize(IndexTupel tuple, int wichtiger, int weniger)
        {
            tuple.Weniger.ShouldBe(weniger);
            tuple.Wichtiger.ShouldBe(wichtiger);
        }

        [Fact]
        public void Gesamtgewichtung_berechne_richtig()
        {
            var gewichtungen = new List<Contracts.Gewichtung>()
            {
                new Contracts.Gewichtung() {StoryIndizes = new List<int> {2, 0, 1}},
                new Contracts.Gewichtung() {StoryIndizes = new List<int> {1, 2, 0}},
                new Contracts.Gewichtung() {StoryIndizes = new List<int> {2, 1, 0}},
            };

            var sut = new Gewichtung();

            var result = sut.Gesamtgewichtung_berechne(gewichtungen);

            result.StoryIndizes.ShouldBe(new[] {2, 1, 0});
        }
    }
}