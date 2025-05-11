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
    bool showAttack = true;

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
            EditorGUILayout.Space();
            item.model = (GameObject)EditorGUILayout.ObjectField("Model", item.model, typeof(GameObject), false);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Stats Group
        showStats = EditorGUILayout.Foldout(showStats, "Stats");
        if (showStats)
        {
            EditorGUI.indentLevel++;
            item.value = EditorGUILayout.IntField("Value", item.value);
            item.weight = EditorGUILayout.FloatField("Weight", item.weight);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

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

                EditorGUILayout.LabelField("Effect", EditorStyles.boldLabel);

                EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();

                EditorGUI.indentLevel++;

                for (int i = 0; i < item.consumeEffect.Count; i++)
                {
                    item.consumeEffect[i].modify_ID = EditorGUILayout.TextField("Key", item.consumeEffect[i].modify_ID);
                    item.consumeEffect[i].modify_Type = (ValueType)EditorGUILayout.EnumPopup("Type", item.consumeEffect[i].modify_Type);
                    switch (item.consumeEffect[i].modify_Type)
                    {
                        case ValueType.Int:
                            item.consumeEffect[i].modify_IntValue = EditorGUILayout.IntField("Value", item.consumeEffect[i].modify_IntValue);
                            break;
                        case ValueType.Float:
                            item.consumeEffect[i].modify_FloatValue = EditorGUILayout.FloatField("Value", item.consumeEffect[i].modify_FloatValue);
                            break;
                        case ValueType.Bool:
                            item.consumeEffect[i].modify_BoolValue = EditorGUILayout.Toggle("Value", item.consumeEffect[i].modify_BoolValue);
                            break;
                        default:
                            item.consumeEffect[i].modify_StringValue = EditorGUILayout.TextField("Value", item.consumeEffect[i].modify_StringValue);
                            break;
                    }
                    item.consumeEffect[i].modify_Des = EditorGUILayout.TextField("Des", item.consumeEffect[i].modify_Des);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel--;

                if (GUILayout.Button("Add Effect"))
                {
                    item.consumeEffect.Add(new Modify());
                }

                if (GUILayout.Button("Remove Last"))
                {
                    if (item.consumeEffect.Count > 0)
                        item.consumeEffect.RemoveAt(item.consumeEffect.Count - 1);
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Script", EditorStyles.boldLabel);

                for (int i = 0; i < item.effectsModules.Count; i++)
                {
                    item.effectsModules[i] = (MonoScript)EditorGUILayout.ObjectField($"Script {i + 1}", item.effectsModules[i], typeof(MonoScript), false);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
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

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Melee Attack Group
        showAttack = EditorGUILayout.Foldout(showAttack, "Attack");
        if (showAttack)
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
                    if (item.canMelee)
                    {
                        item.hitbox = (GameObject)EditorGUILayout.ObjectField("Hitbox", item.hitbox, typeof(GameObject), false);
                        item.mele_manaConsume = EditorGUILayout.FloatField("Mana Cost", item.mele_manaConsume);
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
                        item.bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", item.bulletSpeed);
                        item.bulletLifetime = EditorGUILayout.FloatField("Bullet Lifetime", item.bulletLifetime);
                        EditorGUILayout.Space();
                        item.spread = EditorGUILayout.FloatField("Bullet Spread", item.spread);
                        item.shooting_manaConsume = EditorGUILayout.FloatField("Mana Cost", item.shooting_manaConsume);
                        EditorGUILayout.Space();
                        item.reload = EditorGUILayout.FloatField("Reload Time", item.reload);
                        item.reload_manaConsume = EditorGUILayout.FloatField("Mana Cost", item.reload_manaConsume);
                        EditorGUILayout.Space();
                        item.clipSize = EditorGUILayout.IntField("Clip Size", item.clipSize);
                        item.fireAmount = EditorGUILayout.IntField("Amount", item.fireAmount);
                        EditorGUILayout.Space();
                        item.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", item.projectile, typeof(GameObject), false);
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
