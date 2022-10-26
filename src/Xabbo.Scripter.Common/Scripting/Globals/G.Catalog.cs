﻿using System;

using Xabbo.Interceptor;
using Xabbo.Core;
using Xabbo.Core.GameData;
using Xabbo.Core.Tasks;

namespace Xabbo.Scripter.Scripting;

public partial class G
{
    /// <summary>
    /// Gets the catalog of the specified type.
    /// </summary>
    /// <param name="type">The type of catalog to retrieve.</param>
    /// <param name="timeout">The time in milliseconds to wait for a response from the server.</param>
    public ICatalog GetCatalog(string type = "NORMAL", int timeout = DEFAULT_TIMEOUT)
        => new GetCatalogTask(Interceptor, type).Execute(timeout, Ct);

    /// <summary>
    /// Gets the builder's club catalog.
    /// </summary>
    /// <param name="timeout">The time in milliseconds to wait for a response from the server.</param>
    public ICatalog GetBcCatalog(int timeout = DEFAULT_TIMEOUT) => GetCatalog("BUILDERS_CLUB", timeout);

    /// <summary>
    /// Gets the specified catalog page from the catalog.
    /// </summary>
    /// <param name="pageId">The ID of the page to retrieve.</param>
    /// <param name="type">The catalog type to retrieve the page from.</param>
    /// <param name="timeout">The time in milliseconds to wait for a response from the server.</param>
    public ICatalogPage GetCatalogPage(int pageId, string type = "NORMAL", int timeout = DEFAULT_TIMEOUT)
        => new GetCatalogPageTask(Interceptor, pageId, type).Execute(timeout, Ct);

    /// <summary>
    /// Gets the specified catalog page from the builder's club catalog.
    /// </summary>
    /// <param name="pageId">The ID of the page to retrieve.</param>
    /// <param name="timeout">The time in milliseconds to wait for a response from the server.</param>
    public ICatalogPage GetBcCatalogPage(int pageId, int timeout = DEFAULT_TIMEOUT)
        => GetCatalogPage(pageId, "BUILDERS_CLUB", timeout);

    /// <summary>
    /// Gets the catalog page corresponding to the specified page node from the catalog.
    /// </summary>
    /// <param name="node">The node of which to load the corresponding catalog page.</param>
    /// <param name="timeout">The time in milliseconds to wait for a response from the server.</param>
    public ICatalogPage GetCatalogPage(ICatalogPageNode node, int timeout = DEFAULT_TIMEOUT)
    {
        ArgumentNullException.ThrowIfNull(node);
        return GetCatalogPage(node.Id, node.Catalog?.Type ?? "NORMAL", timeout);
    }

    /// <summary>
    /// Sends a request to purchase the specified catalog offer.
    /// </summary>
    /// <param name="offer">The offer to purchase.</param>
    /// <param name="count">The number of items to purchase.</param>
    /// <param name="extra">
    /// The extra parameter.
    /// In case of trophies, this is the message to be displayed on the trophy.
    /// For group furni, this is the ID of the group as a string.
    /// </param>
    public void Purchase(ICatalogOffer offer, int count = 1, string extra = "")
    {
        ArgumentNullException.ThrowIfNull(offer);
        ArgumentNullException.ThrowIfNull(offer.Page);

        Purchase(offer.Page.Id, offer.Id, count, extra);
    }

    /// <summary>
    /// Sends a request to purchase the specified catalog offer.
    /// </summary>
    /// <param name="pageId">The ID of the catalog page.</param>
    /// <param name="offerId">The ID of the offer to purchase.</param>
    /// <param name="count">The number of items to purchase.</param>
    /// <param name="extra">
    /// The extra parameter.
    /// In case of trophies, this is the message to be displayed on the trophy.
    /// For group furni, this is the ID of the group as a string.
    /// </param>
    public void Purchase(int pageId, int offerId, int count = 1, string extra = "")
    {
        Interceptor.Send(Out.PurchaseFromCatalog, pageId, offerId, extra, count);
    }

    /// <summary>
    /// Sends a request to purchase the specified catalog offer as a gift.
    /// </summary>
    /// <param name="offer">The catalog offer to purchase.</param>
    /// <param name="recipient">The name of the recipient to send to.</param>
    /// <param name="message">The message to display on the gift.</param>
    /// <param name="extra">
    /// The extra parameter.
    /// In case of trophies, this is the message to be displayed on the trophy.
    /// For group furni, this is the ID of the group as a string.
    /// </param>
    /// <param name="giftFurni">
    /// The gift furni identifier.
    /// If none is specified, a random one from
    /// <c>present_gen</c> to <c>present_gen6</c> will be chosen.
    /// </param>
    /// <param name="box"></param>
    /// <param name="decor"></param>
    public void PurchaseAsGift(ICatalogOffer offer, string recipient,
        string message = "", string extra = "",
        string? giftFurni = null,
        GiftBox box = GiftBox.Basic,
        GiftDecor decor = GiftDecor.None)
    {
        ArgumentNullException.ThrowIfNull(offer);

        if (string.IsNullOrWhiteSpace(giftFurni))
        {
            int n = Rand(7);
            if (n == 0) giftFurni = "present_gen";
            else giftFurni = $"present_gen{n}";
        }

        FurniInfo? giftInfo = FurniData[giftFurni];
        if (giftInfo is null)
        {
            throw new Exception($"Furni does not exist: \"{giftFurni}\".");
        }
        else
        {
            if (giftInfo.Category != FurniCategory.Gift)
            {
                throw new Exception($"Invalid gift furni: \"{giftFurni}\".");
            }
        }

        int pageId = offer.Page?.Id ?? throw new Exception("Failed to get page ID from catalog offer.");

        Interceptor.Send(Out.PurchaseFromCatalogAsGift,
            pageId, offer.Id, extra,
            recipient, message,
            giftInfo.Kind, (int)box, (int)decor,
            true
        );
    }

    public static class GiftFurni
    {
        public const string BasicRed = "present_gen";
        public const string BasicGreen = "present_gen1";
        public const string BasicPurple = "present_gen2";
        public const string BasicOrange = "present_gen3";
        public const string BasicYellow = "present_gen4";
        public const string BasicPink = "present_gen5";
        public const string BasicGray = "present_gen6";

        public const string WrapMaroon = "present_wrap*1";
        public const string WrapWhite = "present_wrap*10";
        public const string WrapOrange = "present_wrap*2";
        public const string WrapPink = "present_wrap*3";
        public const string WrapPeach = "present_wrap*4";
        public const string WrapYellow = "present_wrap*5";
        public const string WrapGreen = "present_wrap*6";
        public const string WrapDarkCyan = "present_wrap*7";
        public const string WrapBlue = "present_wrap*8";
        public const string WrapGray = "present_wrap*9";
    }

    public enum GiftBox
    {
        Basic = -1,
        Royal = 0,
        Imperial = 1,
        Glamor = 2,
        Cardboard = 3,
        Steel = 4,
        IceCube = 5,
        Wooden = 6,
        Valentines = 8
    }

    public enum GiftDecor
    {
        RedSilkKnotRibbon = 0,
        GoldenSilkKnotRibbon = 1,
        BlueSilkKnotRibbon = 2,
        PinkBow = 3,
        GreenBow = 4,
        WhiteBow = 5,
        PlainRibbon = 6,
        OrganicRibbon = 7,
        Suspenders = 8,
        Chains = 9,
        None = 10
    }
}
