using DXStats.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DXStats.Persistence
{
    public class DxStatsDbContext : DbContext
    {
        public DxStatsDbContext(DbContextOptions<DxStatsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Snapshot> Snapshots { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<DayCompletedOrders> DayCompletedOrders { get; set; }
        public DbSet<DayVolume> DayVolumes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Snapshot>()
                .HasMany(s => s.DayVolumes)
                .WithOne(dv => dv.Snapshot);

            modelBuilder.Entity<Snapshot>()
                .HasMany(s => s.DayCompletedOrders)
                .WithOne(dc => dc.Snapshot);

            seedCoins.ForEach(c =>
            {
                modelBuilder.Entity<Coin>().HasData(new Coin
                {
                    Id = c
                });
            });
        }

        static readonly List<string> seedCoins = new List<string>()
        {
                "ABS",
                "AEX",
                "AGM",
                "AMS",
                "APR",
                "ATB",
                "AUS",
                "BAD",
                "BAY",
                "BCH",
                "BCZ",
                "BIT",
                "BITG",
                "BLAST",
                "BLOCK",
                "BSD",
                "BTC",
                "BTDX",
                "BTX",
                "BZX",
                "CARE",
                "CDZC",
                "CHC",
                "CHI",
                "CHN",
                "CIV",
                "CNMC",
                "COLX",
                "CRAVE",
                "CRW",
                "D",
                "DASH",
                "DGB",
                "DIN",
                "DIVI",
                "DMD",
                "DOGE",
                "DOGEC",
                "DSR",
                "DVT",
                "DYN",
                "ECA",
                "EMC",
                "EMC2",
                "ENT",
                "FAIR",
                "FGC",
                "FJC",
                "FLO",
                "GALI",
                "GBX",
                "GEEK",
                "GIN",
                "GLC",
                "GMCN",
                "GXX",
                "HASH",
                "HATCH",
                "HLM",
                "HTML",
                "INN",
                "IOP",
                "IXC",
                "JEW",
                "JIYOX",
                "KLKS",
                "KYDC",
                "KZC",
                "LBC",
                "LPC",
                "LTC",
                "LUX",
                "LYNX",
                "MAC",
                "MERGE",
                "MLM",
                "MNP",
                "MNX",
                "MONA",
                "MRX",
                "MUE",
                "N8V",
                "NIX",
                "NMC",
                "NOR",
                "NORT",
                "NYEX",
                "NYX",
                "ODIN",
                "OHMC",
                "OPCX",
                "ORE",
                "PAC",
                "PHL",
                "PHR",
                "PIVX",
                "POLIS",
                "PURA",
                "QBIC",
                "QTUM",
                "RPD",
                "RVN",
                "SCC",
                "SCN",
                "SCRIBE",
                "SEND",
                "SEQ",
                "SIB",
                "SPK",
                "STAK",
                "SUB1X",
                "SYNX",
                "SYS",
                "TRB",
                "TRC",
                "UFO",
                "UNO",
                "VIA",
                "VITAE",
                "VIVO",
                "VSX",
                "VTC",
                "WGR",
                "XC",
                "XLR",
                "XMCC",
                "XMY",
                "XN",
                "XSN",
                "XVG",
                "XZC",
                "ZNZ"
         };
    }
}
