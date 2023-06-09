﻿SELECT 
	u.Id as UserId
   ,u.UserName
   ,r.Id as RoleId
   ,r.Name
   ,p.Id as PageId
   ,p.Name 

FROM Users u

JOIN UsersRoles ur
ON u.Id = ur.UserId

JOIN Roles r
ON r.Id = ur.RoleId

JOIN RolesPages rp
ON r.Id = rp.RoleId

JOIN Pages p
ON p.Id = rp.PageId


ORDER BY u.Id, r.Id, p.Id