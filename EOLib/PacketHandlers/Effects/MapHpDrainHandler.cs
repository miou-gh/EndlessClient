﻿
using AutomaticTypeMapper;
using EOLib.Domain.Character;
using EOLib.Domain.Extensions;
using EOLib.Domain.Login;
using EOLib.Domain.Notifiers;
using EOLib.IO.Map;
using EOLib.Net;
using EOLib.Net.Handlers;
using System;
using System.Collections.Generic;

namespace EOLib.PacketHandlers.Effects
{
    [AutoMappedType]
    public class MapHpDrainHandler : InGameOnlyPacketHandler
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IEnumerable<IMainCharacterEventNotifier> _mainCharacterEventNotifiers;
        private readonly IEnumerable<IOtherCharacterEventNotifier> _otherCharacterEventNotifiers;
        private readonly IEnumerable<IEffectNotifier> _effectNotifiers;

        public override PacketFamily Family => PacketFamily.Effect;

        public override PacketAction Action => PacketAction.TargetOther;

        public MapHpDrainHandler(IPlayerInfoProvider playerInfoProvider,
                                 ICharacterRepository characterRepository,
                                 IEnumerable<IMainCharacterEventNotifier> mainCharacterEventNotifiers,
                                 IEnumerable<IOtherCharacterEventNotifier> otherCharacterEventNotifiers,
                                 IEnumerable<IEffectNotifier> effectNotifiers)
            : base(playerInfoProvider)
        {
            _characterRepository = characterRepository;
            _mainCharacterEventNotifiers = mainCharacterEventNotifiers;
            _otherCharacterEventNotifiers = otherCharacterEventNotifiers;
            _effectNotifiers = effectNotifiers;
        }

        public override bool HandlePacket(IPacket packet)
        {
            var damage = packet.ReadShort();
            var hp = packet.ReadShort();
            var maxhp = packet.ReadShort();

            _characterRepository.MainCharacter = _characterRepository.MainCharacter.WithDamage(damage, hp == 0);

            foreach (var notifier in _mainCharacterEventNotifiers)
                notifier.NotifyTakeDamage(damage, (int)Math.Round(((double)hp / maxhp) * 100), isHeal: false);

            foreach (var notifier in _effectNotifiers)
                notifier.NotifyMapEffect(MapEffect.HPDrain);

            while (packet.ReadPosition != packet.Length)
            {
                var otherCharacterId = packet.ReadShort();
                var otherCharacterPercentHealth = packet.ReadChar();
                var damageDealt = packet.ReadShort();

                foreach (var notifier in _otherCharacterEventNotifiers)
                    notifier.OtherCharacterTakeDamage(otherCharacterId, otherCharacterPercentHealth, damageDealt, isHeal: false);
            }

            return true;
        }
    }
}
