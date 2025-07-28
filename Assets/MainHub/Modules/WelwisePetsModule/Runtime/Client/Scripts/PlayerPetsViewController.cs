using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WelwisePetsModule.Runtime.Shared.Scripts;

namespace WelwisePetsModule.Runtime.Client.Scripts
{
    public class PlayerPetsViewController
    {
        private List<PetViewController> _petControllers;
        private readonly PetsViewFactory _petsViewFactory;

        private readonly Transform _petsOwnerTransform;
        private readonly int _petsLayer;

        public PlayerPetsViewController(PetsViewFactory petsViewFactory, Transform petsOwnerTransform,
            SelectedPetsData selectedPetsData, int petsLayer = -1)
        {
            _petsViewFactory = petsViewFactory;
            _petsOwnerTransform = petsOwnerTransform;
            _petsLayer = petsLayer;

            UpdatePetsViewAsync(selectedPetsData).Forget();
        }

        public async UniTask UpdatePetsViewAsync(SelectedPetsData selectedPetsData)
        {
            var selectedPetsCount = selectedPetsData.SelectedItemsData.Count;
            
            _petControllers ??= Enumerable.Range(0, selectedPetsCount).Select(i => null as PetViewController).ToList();
            
            for (var i = 0; i < selectedPetsCount; i++)
            {
                var petController = _petControllers[i];
                var data = selectedPetsData.SelectedItemsData[i];
                
                if (petController == null && data.Index == null || data.Index != null && petController != null &&
                    data.Index == petController.ViewConfig.ItemIndex) continue;
                
                if (petController != null)
                    Object.Destroy(petController.ViewTransform.gameObject);
                
                if (data.Index == null)
                {
                    _petControllers[i] = null;
                    continue;
                }
                
                var newInstanceController =
                    await _petsViewFactory.GetCreatedPetAndViewControllerAsync(data.Index, data.OrdinalIndex, _petsOwnerTransform);

                if (_petsLayer != -1)
                    newInstanceController.ViewTransform.gameObject.layer = _petsLayer;
                
                _petControllers[i] = newInstanceController;
            }
        }
    }
}