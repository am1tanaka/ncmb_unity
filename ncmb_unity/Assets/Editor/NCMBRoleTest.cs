﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System;
using System.Reflection;
using NUnit.Framework;
using NCMB;

public class NCMBRoleTest : MonoBehaviour
{

	[TestFixtureSetUp]
	public void Init ()
	{
		NCMBTestSettings.Initialize ();
	}


	/**
     * - 内容：空ロール検索時にユーザーの追加ができる事を確認する
     * - 結果：追加されたユーザーをロールから取得し、ローカルのユーザーとobjectIdが一致すること
     */
	[Test]
	public void addRoleUserTest ()
	{
		// ユーザー作成
		NCMBUser expertUser = new NCMBUser ();
		expertUser.UserName = "expertUser";
		expertUser.Password = "pass";
		expertUser.SignUpAsync ((error) => {
			if (error != null) {
				Assert.Fail (error.ErrorMessage);
			}
		});
		NCMBTestSettings.AwaitAsync ();
		Assert.NotNull (expertUser.ObjectId);

		// ロール作成
		NCMBRole expertPlanRole = new NCMBRole ("expertPlan");
		expertPlanRole.SaveAsync ((error) => {
			if (error != null) {
				Assert.Fail (error.ErrorMessage);
			}
		});
		NCMBTestSettings.AwaitAsync ();
		Assert.NotNull (expertPlanRole.ObjectId);

		// 空のロールを検索
		NCMBRole expertPlan = null;
		NCMBRole.GetQuery ().WhereEqualTo ("roleName", "expertPlan").FindAsync ((roleList, error) => {
			if (error != null) {
				Assert.Fail (error.ErrorMessage);
			} else {
				expertPlan = roleList.FirstOrDefault ();
			}
		});
		NCMBTestSettings.AwaitAsync ();
		Assert.NotNull (expertPlan.ObjectId);

		// 空のロールにユーザーを追加
		expertPlan.Users.Add (expertUser);
		expertPlan.SaveAsync ((error) => {
			if (error != null) {
				Assert.Fail (error.ErrorMessage);
			}
		});
		NCMBTestSettings.AwaitAsync ();

		// ロールを検索
		expertPlan = null;
		NCMBRole.GetQuery ().WhereEqualTo ("roleName", "expertPlan").FindAsync ((roleList, error) => {
			if (error != null) {
				Assert.Fail (error.ErrorMessage);
			} else {
				expertPlan = roleList.FirstOrDefault ();
			}
		});
		NCMBTestSettings.AwaitAsync ();

		// ロールに所属するユーザーを検索
		expertPlan.Users.GetQuery ().FindAsync ((userList, error) => {
			if (error != null) {
				Assert.Fail (error.ErrorMessage);
			} else {
				Assert.AreEqual (expertUser.ObjectId, userList.FirstOrDefault ().ObjectId);
				NCMBTestSettings.CallbackFlag = true;		
				// テストデータ削除
				expertPlan.DeleteAsync ();
				expertUser.DeleteAsync ();	
			}
		});
		NCMBTestSettings.AwaitAsync ();
		Assert.True (NCMBTestSettings.CallbackFlag);
	}
}
