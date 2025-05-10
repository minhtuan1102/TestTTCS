using NUnit.Framework.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    bool showBasicInfo = true;
    bool showStats = true;
    bool showEffect = true;

    bool showAttackStats = true;
    bool showAttackAnimStats = true;
    bool showMeleeStats = true;
    bool showShootingStats = true;

    private Type[] availableScripts;
    private int selectedIndex = 0;
    private string[] scriptNames;

    void OnEnable()
    {
        availableScripts = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MonoScript)))
            .ToArray();

        scriptNames = availableScripts.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        Item item = (Item)target;

        // Basic Info
        showBasicInfo = EditorGUILayout.Foldout(showBasicInfo, "Basic Info");
        if (showBasicInfo)
        {
            EditorGUI.indentLevel++;
            item.itemName = EditorGUILayout.TextField("Item Name", item.itemName);
            item.itemID = EditorGUILayout.TextField("Item ID", item.itemID);
            item._itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", item._itemType);
            item.itemType = item._itemType.ToString();
            //item.itemType = EditorGUILayout.TextField("<Item Type>", item.itemType);
            EditorGUILayout.Space();
            item.itemDescription = EditorGUILayout.TextField("Item Description", item.itemDescription);
            item.isWeapon = EditorGUILayout.Toggle("Weapon", item.isWeapon);
            EditorGUILayout.Space();
            item.icon = (Sprite)EditorGUILayout.ObjectField("Icon", item.icon, typeof(Sprite), false);
            EditorGUI.indentLevel--;
        }

        // Stats Group
        showStats = EditorGUILayout.Foldout(showStats, "Stats");
        if (showStats)
        {
            EditorGUI.indentLevel++;
            item.value = EditorGUILayout.IntField("Value", item.value);
            item.weight = EditorGUILayout.FloatField("Weight", item.weight);
            EditorGUI.indentLevel--;
        }

        // Effect Group
        showEffect = EditorGUILayout.Foldout(showEffect, "Effect");
        if (showEffect)
        {
            EditorGUI.indentLevel++;
            item.isConsumable = EditorGUILayout.Toggle("Is Consumable", item.isConsumable);
            if (item.isConsumable )
            {
                item.effectDescription = EditorGUILayout.TextField("Effect Description", item.effectDescription);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Script", EditorStyles.boldLabel);

                for (int i = 0; i < item.effectsModules.Count; i++)
                {
                    item.effectsModules[i] = (MonoScript)EditorGUILayout.ObjectField($"Script {i + 1}", item.effectsModules[i], typeof(MonoScript), false);
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Add Script Slot"))
                {
                    item.effectsModules.Add(null);
                }

                if (GUILayout.Button("Remove Last"))
                {
                    if (item.effectsModules.Count > 0)
                        item.effectsModules.RemoveAt(item.effectsModules.Count - 1);
                }
            }
            EditorGUI.indentLevel--;
        }

        // Melee Attack Group
        showEffect = EditorGUILayout.Foldout(showEffect, "Melee");
        if (showEffect)
        {
            EditorGUI.indentLevel++;
            item.canAttack = EditorGUILayout.Toggle("Can Attack", item.canAttack);
            if (item.canAttack)
            {

                showAttackStats = EditorGUILayout.Foldout(showAttackStats, "Basic");

                if (showAttackStats)
                {
                    EditorGUI.indentLevel++;
                    item.damage = EditorGUILayout.FloatField("Damage", item.damage);
                    item.cooldown = EditorGUILayout.FloatField("Cooldown", item.cooldown);
                    item.manaConsume = EditorGUILayout.FloatField("Mana Cost", item.manaConsume);
                    EditorGUI.indentLevel--;
                }

                showAttackAnimStats = EditorGUILayout.Foldout(showAttackAnimStats, "Animation");

                if (showAttackAnimStats)
                {
                    EditorGUI.indentLevel++;
                    item.swing = EditorGUILayout.FloatField("Swing Angle", item.swing);
                    item.swingOffset = EditorGUILayout.FloatField("Swing Angel Offset", item.swingOffset);
                    EditorGUILayout.Space();
                    item.recoil = EditorGUILayout.FloatField("Recoil", item.recoil);
                    EditorGUI.indentLevel--;
                }

                showMeleeStats = EditorGUILayout.Foldout(showMeleeStats, "Melee");

                if (showMeleeStats)
                {
                    EditorGUI.indentLevel++;
                    item.canMelee = EditorGUILayout.Toggle("Can Melee", item.canMelee);
                    if (item.canShoot)
                    {
                        item.hitbox = (Transform)EditorGUILayout.ObjectField("Hitbox", item.hitbox, typeof(Transform), false);
                    }
                    EditorGUI.indentLevel--;
                }

                showShootingStats = EditorGUILayout.Foldout(showShootingStats, "Shooting");

                if (showShootingStats)
                {
                    EditorGUI.indentLevel++;
                    item.canShoot = EditorGUILayout.Toggle("Can Shoot", item.canShoot);
                    if (item.canShoot)
                    {
                        item.reload = EditorGUILayout.FloatField("Reload Time", item.reload);
                        item.clipSize = EditorGUILayout.IntField("Clip Size", item.clipSize);
                        item.fireAmount = EditorGUILayout.IntField("Amount", item.fireAmount);
                        item.spread = EditorGUILayout.FloatField("Spread", item.spread);
                        EditorGUILayout.Space();
                        item.projectile = (Transform)EditorGUILayout.ObjectField("Projectile", item.hitbox, typeof(Transform), false);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUI.indentLevel--;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(item);
        }
    }
}
