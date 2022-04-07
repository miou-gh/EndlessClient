﻿using AutomaticTypeMapper;
using EndlessClient.Dialogs.Factories;
using EOLib.Domain.Character;
using EOLib.Domain.Interact;
using EOLib.Domain.Interact.Quest;
using EOLib.Domain.Interact.Shop;
using EOLib.Domain.NPC;
using EOLib.IO;
using Optional;

namespace EndlessClient.Dialogs.Actions
{
    [AutoMappedType]
    public class InGameDialogActions : IInGameDialogActions, INPCInteractionNotifier
    {
        private readonly IFriendIgnoreListDialogFactory _friendIgnoreListDialogFactory;
        private readonly IPaperdollDialogFactory _paperdollDialogFactory;
        private readonly IQuestStatusDialogFactory _questStatusDialogFactory;
        private readonly IActiveDialogRepository _activeDialogRepository;
        private readonly IShopDataRepository _shopDataRepository;
        private readonly IQuestDataRepository _questDataRepository;
        private readonly IShopDialogFactory _shopDialogFactory;
        private readonly IQuestDialogFactory _questDialogFactory;

        public InGameDialogActions(IFriendIgnoreListDialogFactory friendIgnoreListDialogFactory,
                                   IPaperdollDialogFactory paperdollDialogFactory,
                                   IQuestStatusDialogFactory questStatusDialogFactory,
                                   IShopDialogFactory shopDialogFactory,
                                   IQuestDialogFactory questDialogFactory,
                                   IActiveDialogRepository activeDialogRepository,
                                   IShopDataRepository shopDataRepository,
                                   IQuestDataRepository questDataRepository)
        {
            _friendIgnoreListDialogFactory = friendIgnoreListDialogFactory;
            _paperdollDialogFactory = paperdollDialogFactory;
            _questStatusDialogFactory = questStatusDialogFactory;
            _activeDialogRepository = activeDialogRepository;
            _shopDataRepository = shopDataRepository;
            _questDataRepository = questDataRepository;
            _shopDialogFactory = shopDialogFactory;
            _questDialogFactory = questDialogFactory;
        }

        public void ShowFriendListDialog()
        {
            _activeDialogRepository.FriendIgnoreDialog.MatchNone(() =>
            {
                var dlg = _friendIgnoreListDialogFactory.Create(isFriendList: true);
                dlg.DialogClosed += (_, _) => _activeDialogRepository.FriendIgnoreDialog = Option.None<ScrollingListDialog>();
                _activeDialogRepository.FriendIgnoreDialog = Option.Some(dlg);

                dlg.Show();
            });
        }

        public void ShowIgnoreListDialog()
        {
            _activeDialogRepository.FriendIgnoreDialog.MatchNone(() =>
            {
                var dlg = _friendIgnoreListDialogFactory.Create(isFriendList: false);
                dlg.DialogClosed += (_, _) => _activeDialogRepository.FriendIgnoreDialog = Option.None<ScrollingListDialog>();
                _activeDialogRepository.FriendIgnoreDialog = Option.Some(dlg);

                dlg.Show();
            });
        }

        public void ShowQuestStatusDialog()
        {
            _activeDialogRepository.QuestStatusDialog.MatchNone(() =>
            {
                var dlg = _questStatusDialogFactory.Create();
                dlg.DialogClosed += (_, _) => _activeDialogRepository.QuestStatusDialog = Option.None<QuestStatusDialog>();
                _activeDialogRepository.QuestStatusDialog = Option.Some(dlg);

                dlg.Show();
            });
        }

        public void ShowPaperdollDialog(ICharacter character, bool isMainCharacter)
        {
            _activeDialogRepository.PaperdollDialog.MatchNone(() =>
            {
                var dlg = _paperdollDialogFactory.Create(character, isMainCharacter);
                dlg.DialogClosed += (_, _) => _activeDialogRepository.PaperdollDialog = Option.None<PaperdollDialog>();
                _activeDialogRepository.PaperdollDialog = Option.Some(dlg);

                dlg.Show();
            });
        }

        public void NotifyInteractionFromNPC(NPCType npcType)
        {
            // originally, these methods were called directly from NPCInteractionController
            // however, this resulted in empty responses (e.g. no shop or quest) showing an empty dialog
            // instead, wait for the response packet to notify this class and then show the dialog
            //    once data has been received from the server
            switch (npcType)
            {
                case NPCType.Shop: ShowShopDialog(); break;
                case NPCType.Quest: ShowQuestDialog(); break;
            }
        }

        public void ShowShopDialog()
        {
            _activeDialogRepository.ShopDialog.MatchNone(() =>
            {
                var dlg = _shopDialogFactory.Create();
                dlg.DialogClosed += (_, _) =>
                {
                    _activeDialogRepository.ShopDialog = Option.None<ShopDialog>();
                    _shopDataRepository.ResetState();
                };
                _activeDialogRepository.ShopDialog = Option.Some(dlg);

                dlg.Show();
            });
        }

        public void ShowQuestDialog()
        {
            _activeDialogRepository.QuestDialog.MatchNone(() =>
            {
                var dlg = _questDialogFactory.Create();
                dlg.DialogClosed += (_, _) =>
                {
                    _activeDialogRepository.QuestDialog = Option.None<QuestDialog>();
                    _questDataRepository.ResetState();
                };
                _activeDialogRepository.QuestDialog = Option.Some(dlg);

                dlg.Show();
            });
        }
    }

    public interface IInGameDialogActions
    {
        void ShowFriendListDialog();

        void ShowIgnoreListDialog();

        void ShowQuestStatusDialog();

        void ShowPaperdollDialog(ICharacter character, bool isMainCharacter);

        void ShowShopDialog();

        void ShowQuestDialog();
    }
}
