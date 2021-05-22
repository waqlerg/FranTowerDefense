﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [Header("Pog Shooter Upgrades")]
    public float rangeUpgrade = 2f;
    public float fireRateUpgrade = 0.70f;

    [Header("Other")]
    public float sellMultiplier;

    [Header("Game Object Reference")]
    public GameObject upgradeAssets;
    public Text sellAmount;

    private GameObject upgradeUI;
    private GameObject currentTower;
    private Upgrade upgrade;
    private GameObject player;

    private string towerType;
    private int refund;
    private int upgradeTotalPrice;

    void Awake()
    {
        upgradeUI = GameObject.Find("Buttons");
        player = GameObject.Find("Player");
        refund = 0;
        upgradeTotalPrice = 0;
    }

    void Update()
    {
        currentTower = GameObject.FindGameObjectWithTag("SelectedTower");

        if (currentTower != null && currentTower.GetComponent<PlaceTower>().placedTower)
        {
            towerType = currentTower.GetComponent<Tower>().towerType;

            SetUpgrades(currentTower.GetComponent<Tower>().GetUpgradeInstance());
            ShowUpgradeUI();
            CalculateRefund();

            //Send specific tower and upgrade info to UpgradeAssets to update images and text
            int treeOneLevel = upgrade.GetUpgradeLevel(1);
            int treeTwoLevel = upgrade.GetUpgradeLevel(2);
            int priceOne = GetUpgradePrice(1, treeOneLevel);
            int priceTwo = GetUpgradePrice(2, treeTwoLevel);

            upgradeAssets.GetComponent<UpgradeAssets>().UpdateImagesAndText(towerType, upgrade.GetUpgradeLevel(1), upgrade.GetUpgradeLevel(2), priceOne, priceTwo, refund);
            ToggleUpgradeButton(treeOneLevel, treeTwoLevel);
        }
        else
        {
            HideUpgradeUI();
        }
    }

    private void SetUpgrades(Upgrade upgrade)
    {
        this.upgrade = upgrade;
    }

    private int GetUpgradePrice(int tree, int upgradeLevel)
    {
        if (tree == 1)
        {
            if (upgradeLevel == 1)
            {
                return currentTower.GetComponent<Tower>().upgrade11Price;
            }
            else if (upgradeLevel == 2)
            {
                return currentTower.GetComponent<Tower>().upgrade12Price;
            }
            else
            {
                return 0;
            }
        }
        else if (tree == 2)
        {
            if (upgradeLevel == 1)
            {
                return currentTower.GetComponent<Tower>().upgrade21Price;
            }
            else if (upgradeLevel == 2)
            {
                return currentTower.GetComponent<Tower>().upgrade22Price;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }

    public void UpgradeButton(int tree)
    {
        int upgradeLevel = upgrade.GetUpgradeLevel(tree); //The "1" is the specific tree, in this case, the first tree of upgrades
        int price = GetUpgradePrice(tree, upgradeLevel);

        if (AffordUpgrade(price))
        {
            SubtractMoney(price);
            upgradeTotalPrice += price; //This adds the upgrade price to the total value of the tower; specifically for the CalculateRefund() function
            UnlockUpgradeForSpecificTower(currentTower.GetComponent<Tower>().towerType, tree, upgradeLevel);
        }
    }

    public void UnlockUpgradeForSpecificTower(string towerType, int tree, int upgradeLevel)
    {
        if(towerType == "PogShooter")
        {
            PogShooterUpgrades(tree, upgradeLevel);
        }
    }

    public void SellTower()
    {
        player.GetComponent<Player>().money += refund;
        Destroy(currentTower);
    }

    private bool AffordUpgrade(int price)
    {
        if (player.GetComponent<Player>().money - price >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CalculateRefund()
    {
        int priceWithUpgrades = currentTower.GetComponent<Tower>().price + upgradeTotalPrice;
        refund = (int)(priceWithUpgrades * sellMultiplier);
    }

    void ToggleUpgradeButton(int treeOneLevel, int treeTwoLevel)
    {
        if(treeOneLevel > 2) 
        {
            upgradeUI.transform.GetChild(0).GetComponent<Button>().interactable = false;
        }
        else
        {
            upgradeUI.transform.GetChild(0).GetComponent<Button>().interactable = true;
        }
        if(treeTwoLevel > 2)
        {
            upgradeUI.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }
        else
        {
            upgradeUI.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }
    }

    void ShowUpgradeUI()
    {
        upgradeUI.GetComponent<CanvasGroup>().interactable = true;
        upgradeUI.GetComponent<CanvasGroup>().alpha = 1;
    }

    void HideUpgradeUI()
    {
        upgradeUI.GetComponent<CanvasGroup>().interactable = false;
        upgradeUI.GetComponent<CanvasGroup>().alpha = 0;
    }

    /* Upgrade Methods */

    void PogShooterUpgrades(int tree, int upgradeLevel)
    {
        if(tree == 1 && upgradeLevel == 1)
        {
            UpgradeRange(rangeUpgrade);
        }
        if(tree == 1 && upgradeLevel == 2)
        {
            
        }
        if(tree == 2 && upgradeLevel == 1)
        {
            UpgradeFireRate(fireRateUpgrade);
        }
        if(tree == 2 && upgradeLevel == 2)
        {

        }


        upgrade.IncrementUpgradeLevel(tree);
    }

    /* */

    void SubtractMoney(int price)
    {
        player.GetComponent<Player>().money -= price;
    }

    void UpgradeRange(float range)
    {
        currentTower.GetComponent<Tower>().SetRange(range);
    }

    void UpgradeFireRate(float fireRate)
    {
        currentTower.GetComponent<Tower>().SetFireRate(fireRate);
    }

}