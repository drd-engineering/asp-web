namespace System.Based.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hehe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnnotateType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20, unicode: false),
                        Descr = c.String(maxLength: 500, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DocumentAnnotate",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DocumentId = c.Long(nullable: false),
                        Page = c.Int(nullable: false),
                        AnnotateTypeId = c.Int(nullable: false),
                        LeftPos = c.Double(),
                        TopPos = c.Double(),
                        WidthPos = c.Double(),
                        HeightPos = c.Double(),
                        Color = c.String(maxLength: 50, unicode: false),
                        BackColor = c.String(maxLength: 50, unicode: false),
                        Data = c.String(maxLength: 8000, unicode: false),
                        Data2 = c.String(maxLength: 8000, unicode: false),
                        Rotation = c.Int(nullable: false),
                        ScaleX = c.Double(nullable: false),
                        ScaleY = c.Double(nullable: false),
                        TransX = c.Double(nullable: false),
                        TransY = c.Double(nullable: false),
                        StrokeWidth = c.Double(nullable: false),
                        Opacity = c.Double(nullable: false),
                        CreatorId = c.Long(),
                        AnnotateId = c.Long(),
                        Flag = c.Int(nullable: false),
                        FlagCode = c.String(maxLength: 20, unicode: false),
                        FlagDate = c.DateTime(),
                        FlagImage = c.String(maxLength: 100, unicode: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AnnotateType", t => t.AnnotateTypeId)
                .ForeignKey("dbo.Document", t => t.DocumentId)
                .Index(t => t.DocumentId)
                .Index(t => t.AnnotateTypeId);
            
            CreateTable(
                "dbo.Document",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 500, unicode: false),
                        Descr = c.String(maxLength: 8000, unicode: false),
                        FileName = c.String(maxLength: 100, unicode: false),
                        FileNameOri = c.String(maxLength: 100, unicode: false),
                        ExtFile = c.String(maxLength: 20, unicode: false),
                        FileFlag = c.Int(nullable: false),
                        FileSize = c.Int(nullable: false),
                        MaxPrint = c.Int(nullable: false),
                        MaxDownload = c.Int(nullable: false),
                        ExpiryDay = c.Int(nullable: false),
                        Version = c.String(maxLength: 20, unicode: false),
                        CompanyId = c.Long(nullable: false),
                        MemberFolderId = c.Long(nullable: false),
                        CreatorId = c.Long(),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        Contact = c.String(maxLength: 50, unicode: false),
                        Phone = c.String(maxLength: 20, unicode: false),
                        Email = c.String(maxLength: 100, unicode: false),
                        Descr = c.String(maxLength: 8000, unicode: false),
                        Address = c.String(maxLength: 1000, unicode: false),
                        PointLocation = c.String(maxLength: 1000, unicode: false),
                        Latitude = c.Decimal(nullable: false, precision: 12, scale: 9),
                        Longitude = c.Decimal(nullable: false, precision: 12, scale: 9),
                        CountryCode = c.String(maxLength: 10, unicode: false),
                        CountryName = c.String(maxLength: 50, unicode: false),
                        AdminArea = c.String(maxLength: 50, unicode: false),
                        SubAdminArea = c.String(maxLength: 50, unicode: false),
                        Locality = c.String(maxLength: 50, unicode: false),
                        SubLocality = c.String(maxLength: 50, unicode: false),
                        Thoroughfare = c.String(maxLength: 50, unicode: false),
                        SubThoroughfare = c.String(maxLength: 10, unicode: false),
                        PostalCode = c.String(maxLength: 5, unicode: false),
                        Image1 = c.String(maxLength: 100, unicode: false),
                        Image2 = c.String(maxLength: 100, unicode: false),
                        ImageCard = c.String(maxLength: 100, unicode: false),
                        BackColorBar = c.String(maxLength: 20, unicode: false),
                        BackColorPage = c.String(maxLength: 20, unicode: false),
                        CompanyType = c.Int(nullable: false),
                        ImageQrCode = c.String(maxLength: 50, unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Member",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.String(nullable: false, maxLength: 20, unicode: false),
                        MemberTitleId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        Phone = c.String(nullable: false, maxLength: 20, unicode: false),
                        Email = c.String(nullable: false, maxLength: 50, unicode: false),
                        MemberType = c.Int(nullable: false),
                        KtpNo = c.String(maxLength: 50, unicode: false),
                        ImageProfile = c.String(maxLength: 50, unicode: false),
                        ImageQrCode = c.String(maxLength: 50, unicode: false),
                        LastLogin = c.DateTime(),
                        LastLogout = c.DateTime(),
                        ActivationKeyId = c.Long(),
                        Password = c.String(nullable: false, maxLength: 500, unicode: false),
                        CompanyId = c.Long(),
                        UserGroup = c.String(maxLength: 20, unicode: false),
                        ImageSignature = c.String(maxLength: 100, unicode: false),
                        ImageInitials = c.String(maxLength: 100, unicode: false),
                        ImageStamp = c.String(maxLength: 100, unicode: false),
                        ImageKtp1 = c.String(maxLength: 100, unicode: false),
                        ImageKtp2 = c.String(maxLength: 100, unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.MemberTitle", t => t.MemberTitleId)
                .Index(t => t.MemberTitleId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.DocumentMember",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DocumentId = c.Long(nullable: false),
                        MemberId = c.Long(nullable: false),
                        FlagAction = c.Int(nullable: false),
                        CxPrint = c.Int(nullable: false),
                        CxDownload = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Document", t => t.DocumentId)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .Index(t => t.DocumentId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.MemberAccount",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        Title = c.String(nullable: false, maxLength: 20, unicode: false),
                        AccountNo = c.String(nullable: false, maxLength: 50, unicode: false),
                        AccountName = c.String(nullable: false, maxLength: 50, unicode: false),
                        BankId = c.Int(nullable: false),
                        Branch = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bank", t => t.BankId)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .Index(t => t.MemberId)
                .Index(t => t.BankId);
            
            CreateTable(
                "dbo.Bank",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        Logo = c.String(maxLength: 50, unicode: false),
                        BankType = c.Int(nullable: false),
                        UserId = c.String(maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompanyBank",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankId = c.Int(nullable: false),
                        Branch = c.String(nullable: false, maxLength: 100, unicode: false),
                        AccountNo = c.String(nullable: false, maxLength: 50, unicode: false),
                        AccountName = c.String(nullable: false, maxLength: 50, unicode: false),
                        PaymentMethodId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        UserId = c.String(maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bank", t => t.BankId)
                .ForeignKey("dbo.PaymentMethod", t => t.PaymentMethodId)
                .Index(t => t.BankId)
                .Index(t => t.PaymentMethodId);
            
            CreateTable(
                "dbo.MemberTopupPayment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PaymentNo = c.String(nullable: false, maxLength: 20, unicode: false),
                        PaymentDate = c.DateTime(nullable: false, storeType: "date"),
                        TopupDepositId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        CompanyBankId = c.Int(nullable: false),
                        MemberAccountId = c.Long(),
                        PaymentStatus = c.String(nullable: false, maxLength: 2, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompanyBank", t => t.CompanyBankId)
                .ForeignKey("dbo.MemberAccount", t => t.MemberAccountId)
                .ForeignKey("dbo.MemberTopupDeposit", t => t.TopupDepositId)
                .Index(t => t.TopupDepositId)
                .Index(t => t.CompanyBankId)
                .Index(t => t.MemberAccountId);
            
            CreateTable(
                "dbo.MemberTopupDeposit",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TopupNo = c.String(nullable: false, maxLength: 20, unicode: false),
                        TopupDate = c.DateTime(nullable: false, storeType: "date"),
                        MemberId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        PaymentStatus = c.String(nullable: false, maxLength: 2, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.PaymentMethod",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 5, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        Logo = c.String(maxLength: 50, unicode: false),
                        Descr = c.String(maxLength: 1000, unicode: false),
                        UsingType = c.Int(nullable: false),
                        ConfirmType = c.Int(nullable: false),
                        AppZoneAccess = c.String(maxLength: 1000, unicode: false),
                        UserId = c.String(maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberDepositTransfer",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TrfNo = c.String(nullable: false, maxLength: 20, unicode: false),
                        TrfDate = c.DateTime(nullable: false, storeType: "date"),
                        FromId = c.Long(nullable: false),
                        ToId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.FromId)
                .ForeignKey("dbo.Member", t => t.ToId)
                .Index(t => t.FromId)
                .Index(t => t.ToId);
            
            CreateTable(
                "dbo.MemberDepositTrx",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TrxNo = c.String(nullable: false, maxLength: 20, unicode: false),
                        TrxDate = c.DateTime(nullable: false, storeType: "date"),
                        TrxType = c.String(nullable: false, maxLength: 10, unicode: false),
                        TrxId = c.Long(nullable: false),
                        Descr = c.String(nullable: false, maxLength: 500, unicode: false),
                        MemberId = c.Long(nullable: false),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        DbCr = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.MemberInvited",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        InvitedId = c.Long(nullable: false),
                        Status = c.String(nullable: false, maxLength: 2, unicode: false),
                        DateExpiry = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.InvitedId)
                .ForeignKey("dbo.Member", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.MemberId)
                .Index(t => t.InvitedId);
            
            CreateTable(
                "dbo.MemberPermission",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        Flag = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.MemberSubscribe",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberPermissionId = c.Long(nullable: false),
                        CompanyId = c.Long(nullable: false),
                        Flag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.MemberPermission", t => t.MemberPermissionId)
                .Index(t => t.MemberPermissionId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.MemberProject",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberSubscribeId = c.Long(nullable: false),
                        ProjectId = c.Long(nullable: false),
                        Flag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MemberSubscribe", t => t.MemberSubscribeId)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.MemberSubscribeId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.MemberWorkflow",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberProjectId = c.Long(nullable: false),
                        WorkflowId = c.Long(nullable: false),
                        Flag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MemberProject", t => t.MemberProjectId)
                .ForeignKey("dbo.Workflow", t => t.WorkflowId)
                .Index(t => t.MemberProjectId)
                .Index(t => t.WorkflowId);
            
            CreateTable(
                "dbo.MemberRotation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberWorkflowId = c.Long(nullable: false),
                        RotationId = c.Long(nullable: false),
                        Flag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MemberWorkflow", t => t.MemberWorkflowId)
                .ForeignKey("dbo.Rotation", t => t.RotationId)
                .Index(t => t.MemberWorkflowId)
                .Index(t => t.RotationId);
            
            CreateTable(
                "dbo.Rotation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Subject = c.String(nullable: false, maxLength: 100, unicode: false),
                        WorkflowId = c.Long(nullable: false),
                        Status = c.String(nullable: false, maxLength: 2, unicode: false),
                        Remark = c.String(maxLength: 8000, unicode: false),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        MemberId = c.Long(nullable: false),
                        CreatorId = c.Long(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        DateStatus = c.DateTime(),
                        DateStarted = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .ForeignKey("dbo.Workflow", t => t.WorkflowId)
                .Index(t => t.WorkflowId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.RotationMember",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RotationId = c.Long(nullable: false),
                        WorkflowNodeId = c.Long(nullable: false),
                        MemberId = c.Long(),
                        FlagPermission = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .ForeignKey("dbo.Rotation", t => t.RotationId)
                .ForeignKey("dbo.WorkflowNode", t => t.WorkflowNodeId)
                .Index(t => t.RotationId)
                .Index(t => t.WorkflowNodeId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.WorkflowNode",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WorkflowId = c.Long(nullable: false),
                        MemberId = c.Long(),
                        SymbolId = c.Int(nullable: false),
                        Caption = c.String(maxLength: 100, unicode: false),
                        Info = c.String(maxLength: 1000, unicode: false),
                        Operator = c.String(maxLength: 10, unicode: false),
                        Value = c.String(maxLength: 50, unicode: false),
                        PosLeft = c.String(maxLength: 10, unicode: false),
                        PosTop = c.String(maxLength: 10, unicode: false),
                        Width = c.String(maxLength: 10, unicode: false),
                        Height = c.String(maxLength: 10, unicode: false),
                        TextColor = c.String(maxLength: 20, unicode: false),
                        BackColor = c.String(maxLength: 20, unicode: false),
                        Flag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .ForeignKey("dbo.Symbol", t => t.SymbolId)
                .ForeignKey("dbo.Workflow", t => t.WorkflowId)
                .Index(t => t.WorkflowId)
                .Index(t => t.MemberId)
                .Index(t => t.SymbolId);
            
            CreateTable(
                "dbo.RotationNode",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RotationId = c.Long(nullable: false),
                        WorkflowNodeId = c.Long(nullable: false),
                        PrevWorkflowNodeId = c.Long(),
                        SenderRotationNodeId = c.Long(),
                        MemberId = c.Long(nullable: false),
                        Value = c.String(maxLength: 100, unicode: false),
                        Status = c.String(nullable: false, maxLength: 2, unicode: false),
                        DateRead = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .ForeignKey("dbo.Rotation", t => t.RotationId)
                .ForeignKey("dbo.RotationNode", t => t.SenderRotationNodeId)
                .ForeignKey("dbo.WorkflowNode", t => t.WorkflowNodeId)
                .Index(t => t.RotationId)
                .Index(t => t.WorkflowNodeId)
                .Index(t => t.SenderRotationNodeId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.RotationNodeDoc",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RotationNodeId = c.Long(nullable: false),
                        DocumentId = c.Long(),
                        FlagAction = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Document", t => t.DocumentId)
                .ForeignKey("dbo.RotationNode", t => t.RotationNodeId)
                .Index(t => t.RotationNodeId)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "dbo.RotationNodeRemark",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RotationNodeId = c.Long(nullable: false),
                        Remark = c.String(nullable: false, maxLength: 8000, unicode: false),
                        DateStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RotationNode", t => t.RotationNodeId)
                .Index(t => t.RotationNodeId);
            
            CreateTable(
                "dbo.RotationNodeUpDoc",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RotationNodeId = c.Long(nullable: false),
                        DocumentUploadId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DocumentUpload", t => t.DocumentUploadId)
                .ForeignKey("dbo.RotationNode", t => t.RotationNodeId)
                .Index(t => t.RotationNodeId)
                .Index(t => t.DocumentUploadId);
            
            CreateTable(
                "dbo.DocumentUpload",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FileName = c.String(maxLength: 100, unicode: false),
                        FileNameOri = c.String(maxLength: 100, unicode: false),
                        ExtFile = c.String(maxLength: 20, unicode: false),
                        FileFlag = c.Int(nullable: false),
                        FileSize = c.Int(nullable: false),
                        CreatorId = c.Long(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Symbol",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50, unicode: false),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        Descr = c.String(nullable: false, maxLength: 8000, unicode: false),
                        TextColor = c.String(maxLength: 20, unicode: false),
                        BackColor = c.String(maxLength: 20, unicode: false),
                        Icon = c.String(maxLength: 100, unicode: false),
                        SymbolType = c.Int(nullable: false),
                        ElementName = c.String(maxLength: 50, unicode: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkflowNodeLink",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WorkflowNodeId = c.Long(nullable: false),
                        WorkflowNodeToId = c.Long(nullable: false),
                        Caption = c.String(maxLength: 100, unicode: false),
                        Operator = c.String(maxLength: 10, unicode: false),
                        Value = c.String(maxLength: 20, unicode: false),
                        SymbolId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Symbol", t => t.SymbolId)
                .ForeignKey("dbo.WorkflowNode", t => t.WorkflowNodeId)
                .ForeignKey("dbo.WorkflowNode", t => t.WorkflowNodeToId)
                .Index(t => t.WorkflowNodeId)
                .Index(t => t.WorkflowNodeToId)
                .Index(t => t.SymbolId);
            
            CreateTable(
                "dbo.Workflow",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        Descr = c.String(nullable: false, maxLength: 8000, unicode: false),
                        ProjectId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatorId = c.Long(),
                        IsTemplate = c.Boolean(nullable: false),
                        WfType = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        Descr = c.String(nullable: false, maxLength: 8000, unicode: false),
                        CompanyId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatorId = c.Long(),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.MemberPlanRequest",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        SubscriptTypeId = c.Int(nullable: false),
                        RotationCount = c.Int(nullable: false),
                        FlowActivityCount = c.Int(nullable: false),
                        StorageSize = c.Long(nullable: false),
                        DrDriveSize = c.Long(nullable: false),
                        ExpiryDocDay = c.Int(nullable: false),
                        PackageExpiryDay = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 2, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .ForeignKey("dbo.SubscriptType", t => t.SubscriptTypeId)
                .Index(t => t.MemberId)
                .Index(t => t.SubscriptTypeId);
            
            CreateTable(
                "dbo.SubscriptType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeCode = c.String(nullable: false, maxLength: 10, unicode: false),
                        ClassName = c.String(nullable: false, maxLength: 100, unicode: false),
                        Descr = c.String(maxLength: 1000, unicode: false),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        PriceUnitCode = c.String(nullable: false, maxLength: 10, unicode: false),
                        PriceUnitDescr = c.String(nullable: false, maxLength: 50, unicode: false),
                        RotationCount = c.Int(nullable: false),
                        RotationPrice = c.Decimal(nullable: false, storeType: "money"),
                        FlowActivityCount = c.Int(nullable: false),
                        FlowActivityPrice = c.Decimal(nullable: false, storeType: "money"),
                        StorageSize = c.Long(nullable: false),
                        StoragePrice = c.Decimal(nullable: false, storeType: "money"),
                        DrDriveSize = c.Long(nullable: false),
                        DrDrivePrice = c.Decimal(nullable: false, storeType: "money"),
                        ExpiryDocDay = c.Int(nullable: false),
                        PackageExpiryDay = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberPlan",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        SubscriptTypeId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        PriceUnitCode = c.String(nullable: false, maxLength: 10, unicode: false),
                        PriceUnitDescr = c.String(nullable: false, maxLength: 50, unicode: false),
                        RotationCount = c.Int(nullable: false),
                        RotationPrice = c.Decimal(nullable: false, storeType: "money"),
                        RotationCountAdd = c.Int(nullable: false),
                        RotationCountUsed = c.Int(nullable: false),
                        FlowActivityCount = c.Int(nullable: false),
                        FlowActivityPrice = c.Decimal(nullable: false, storeType: "money"),
                        FlowActivityCountAdd = c.Int(nullable: false),
                        FlowActivityCountUsed = c.Int(nullable: false),
                        StorageSize = c.Long(nullable: false),
                        StoragePrice = c.Decimal(nullable: false, storeType: "money"),
                        StorageSizeAdd = c.Long(nullable: false),
                        StorageSizeUsed = c.Long(nullable: false),
                        DrDriveSize = c.Long(nullable: false),
                        DrDrivePrice = c.Decimal(nullable: false, storeType: "money"),
                        DrDriveSizeAdd = c.Long(nullable: false),
                        DrDriveSizeUsed = c.Long(nullable: false),
                        ExpiryDocDay = c.Int(nullable: false),
                        PackageExpiryDay = c.Int(nullable: false),
                        DrDriveExpiryDay = c.Int(nullable: false),
                        ValidPackage = c.DateTime(),
                        ValidDrDrive = c.DateTime(),
                        IsDefault = c.Boolean(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .ForeignKey("dbo.SubscriptType", t => t.SubscriptTypeId)
                .Index(t => t.MemberId)
                .Index(t => t.SubscriptTypeId);
            
            CreateTable(
                "dbo.MemberPlanExtra",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberPlanId = c.Long(nullable: false),
                        RotationCount = c.Int(nullable: false),
                        RotationPrice = c.Decimal(nullable: false, storeType: "money"),
                        FlowActivityCount = c.Int(nullable: false),
                        FlowActivityPrice = c.Decimal(nullable: false, storeType: "money"),
                        StorageSize = c.Long(nullable: false),
                        StoragePrice = c.Decimal(nullable: false, storeType: "money"),
                        DrDriveSize = c.Long(nullable: false),
                        DrDrivePrice = c.Decimal(nullable: false, storeType: "money"),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MemberPlan", t => t.MemberPlanId)
                .Index(t => t.MemberPlanId);
            
            CreateTable(
                "dbo.MemberSignHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        ImageSign = c.String(nullable: false, maxLength: 100, unicode: false),
                        SignType = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Member", t => t.MemberId)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.MemberTitle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Message",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FromId = c.Long(nullable: false),
                        ToId = c.Long(nullable: false),
                        BroadcastMessageId = c.Long(),
                        TextMessage = c.String(nullable: false, maxLength: 8000, unicode: false),
                        MessageType = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateOpened = c.DateTime(),
                        DateReplied = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BroadcastMessage", t => t.BroadcastMessageId)
                .ForeignKey("dbo.Member", t => t.FromId)
                .ForeignKey("dbo.Member", t => t.ToId)
                .Index(t => t.FromId)
                .Index(t => t.ToId)
                .Index(t => t.BroadcastMessageId);
            
            CreateTable(
                "dbo.BroadcastMessage",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 200, unicode: false),
                        TextMessage = c.String(nullable: false, maxLength: 8000, unicode: false),
                        GroupType = c.Int(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stamp",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompanyId = c.Long(nullable: false),
                        Descr = c.String(maxLength: 1000, unicode: false),
                        StampFile = c.String(nullable: false, maxLength: 100, unicode: false),
                        CreatorId = c.Long(),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ApplConfig",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20, unicode: false),
                        Descr = c.String(nullable: false, maxLength: 50, unicode: false),
                        Value = c.String(nullable: false, maxLength: 1000, unicode: false),
                        Flag = c.Short(nullable: false),
                        DataType = c.String(nullable: false, maxLength: 20, unicode: false),
                        DataLength = c.Int(nullable: false),
                        InitialValue = c.String(maxLength: 1000, unicode: false),
                        UserID = c.String(nullable: false, maxLength: 20, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DrDrive",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Descr = c.String(maxLength: 8000, unicode: false),
                        FileName = c.String(maxLength: 100, unicode: false),
                        FileNameOri = c.String(maxLength: 100, unicode: false),
                        ExtFile = c.String(maxLength: 20, unicode: false),
                        FileFlag = c.Int(nullable: false),
                        FileSize = c.Long(nullable: false),
                        CxDownload = c.Int(nullable: false),
                        MemberFolderId = c.Long(nullable: false),
                        MemberId = c.Long(),
                        DocumentId = c.Long(),
                        DocumentUploadId = c.Long(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DrDriveType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Size = c.Long(nullable: false),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        ExpiryDay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FaspayCreditStatus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        PAYMENT_METHOD = c.String(maxLength: 1, unicode: false),
                        MERCHANTID = c.String(maxLength: 30, unicode: false),
                        MERCHANT_TRANID = c.String(maxLength: 100, unicode: false),
                        ERR_CODE = c.String(maxLength: 10, unicode: false),
                        ERR_DESC = c.String(maxLength: 250, unicode: false),
                        USR_CODE = c.String(maxLength: 4, unicode: false),
                        USR_MSG = c.String(maxLength: 200, unicode: false),
                        TXN_STATUS = c.String(maxLength: 3, unicode: false),
                        CUSTNAME = c.String(maxLength: 120, unicode: false),
                        DESCRIPTION = c.String(maxLength: 100, unicode: false),
                        CURRENCYCODE = c.String(maxLength: 3, unicode: false),
                        AMOUNT = c.Decimal(precision: 10, scale: 2, storeType: "numeric"),
                        SIGNATURE = c.String(maxLength: 40, unicode: false),
                        EUI = c.String(maxLength: 3, unicode: false),
                        TRANSACTIONID = c.Int(),
                        TRANSACTIONTYPE = c.String(maxLength: 2, unicode: false),
                        MPARAM1 = c.String(maxLength: 200, unicode: false),
                        MPARAM2 = c.String(maxLength: 200, unicode: false),
                        ACQUIRER_ID = c.String(maxLength: 30, unicode: false),
                        TRANDATE = c.String(maxLength: 19, unicode: false),
                        IS_BLACKLISTED = c.String(maxLength: 5, unicode: false),
                        FRAUDRISKLEVEL = c.Int(),
                        FRAUDRISKSCORE = c.Decimal(precision: 10, scale: 2, storeType: "numeric"),
                        POINT_USED = c.Int(),
                        POINT_AMOUNT = c.Decimal(precision: 10, scale: 2, storeType: "numeric"),
                        PAYMENT_AMOUNT = c.Decimal(precision: 10, scale: 2, storeType: "numeric"),
                        POINT_BALANCE = c.Decimal(precision: 10, scale: 2, storeType: "numeric"),
                        EXCEED_HIGH_RISK = c.String(maxLength: 5, unicode: false),
                        CARDTYPE = c.String(maxLength: 5, unicode: false),
                        CARD_NO_PARTIAL = c.String(maxLength: 20, unicode: false),
                        CARDNAME = c.String(maxLength: 100, unicode: false),
                        is_on_us = c.String(maxLength: 3, unicode: false),
                        ACQUIRER_BANK = c.String(maxLength: 3, unicode: false),
                        WHITELIST_CARD = c.String(maxLength: 3, unicode: false),
                        BANK_RES_CODE = c.String(maxLength: 20, unicode: false),
                        BANK_RES_MSG = c.String(maxLength: 250, unicode: false),
                        AUTH_ID = c.String(maxLength: 20, unicode: false),
                        BANK_REFERENCE = c.String(maxLength: 100, unicode: false),
                        INSTALLMENT_CODE = c.String(maxLength: 10, fixedLength: true),
                        INSTALLMENT_TERM = c.String(maxLength: 20, unicode: false),
                        INSTALLMENT_MONTHLY = c.Decimal(precision: 10, scale: 2, storeType: "numeric"),
                        INSTALLMENT_LAST = c.Decimal(precision: 10, scale: 2, storeType: "numeric"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FaspayDebitStatus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        request = c.String(maxLength: 1000, unicode: false),
                        trx_id = c.String(maxLength: 1000, unicode: false),
                        merchant_id = c.String(maxLength: 1000, unicode: false),
                        merchant = c.String(maxLength: 1000, unicode: false),
                        bill_no = c.String(maxLength: 1000, unicode: false),
                        payment_reff = c.String(maxLength: 1000, unicode: false),
                        payment_date = c.String(maxLength: 1000, unicode: false),
                        payment_status_code = c.String(maxLength: 1000, unicode: false),
                        payment_status_desc = c.String(maxLength: 1000, unicode: false),
                        signature = c.String(maxLength: 1000, unicode: false),
                        amount = c.String(maxLength: 1000, unicode: false),
                        payment_total = c.String(maxLength: 1000, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FaspayPayment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PayId = c.Long(nullable: false),
                        PayType = c.String(nullable: false, maxLength: 20, unicode: false),
                        TrxId = c.Long(nullable: false),
                        TrxNo = c.String(nullable: false, maxLength: 50, unicode: false),
                        TrxType = c.String(nullable: false, maxLength: 5, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberFolder",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        Descr = c.String(maxLength: 250, unicode: false),
                        FolderType = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberHitLog",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        DataHitId = c.Long(nullable: false),
                        DataHitType = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descr = c.String(nullable: false, maxLength: 50, unicode: false),
                        Info = c.String(maxLength: 1000, unicode: false),
                        BitValue = c.Int(nullable: false),
                        UserId = c.String(maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Descr = c.String(nullable: false, maxLength: 4000),
                        NewsType = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NewsDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        NewsId = c.Long(nullable: false),
                        Image = c.String(nullable: false, maxLength: 50, unicode: false),
                        Descr = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.News", t => t.NewsId)
                .Index(t => t.NewsId);
            
            CreateTable(
                "dbo.NewsType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descr = c.String(nullable: false, maxLength: 50, unicode: false),
                        Info = c.String(maxLength: 1000, unicode: false),
                        BitValue = c.Int(nullable: false),
                        UserId = c.String(maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NewsVideo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20, unicode: false),
                        Title = c.String(maxLength: 500),
                        Descr = c.String(maxLength: 4000),
                        ChannelId = c.String(maxLength: 100, unicode: false),
                        ChannelTitle = c.String(maxLength: 100, unicode: false),
                        CategoryId = c.Long(),
                        DatePublished = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 2, unicode: false),
                        Descr = c.String(nullable: false, maxLength: 20, unicode: false),
                        UserId = c.String(maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PodCast",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Descr = c.String(maxLength: 4000),
                        Duration = c.Int(nullable: false),
                        Image = c.String(nullable: false, maxLength: 100, unicode: false),
                        AudioFileName = c.String(nullable: false, maxLength: 100, unicode: false),
                        FileNameOri = c.String(maxLength: 100, unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RotationNodeLog",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RotationNodeId = c.Long(nullable: false),
                        Descr = c.String(nullable: false, maxLength: 8000, unicode: false),
                        Status = c.String(nullable: false, maxLength: 2, unicode: false),
                        DateStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StatusCode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 2, unicode: false),
                        Descr = c.String(nullable: false, maxLength: 100, unicode: false),
                        TextColor = c.String(maxLength: 20, unicode: false),
                        BackColor = c.String(maxLength: 20, unicode: false),
                        Icon = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubscriptExtraType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        RotationCount = c.Int(nullable: false),
                        FlowActivityCount = c.Int(nullable: false),
                        StorageSize = c.Long(nullable: false),
                        DrDriveSize = c.Long(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.sysdiagrams",
                c => new
                    {
                        diagram_id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 128),
                        principal_id = c.Int(nullable: false),
                        version = c.Int(),
                        definition = c.Binary(maxLength: 8000),
                    })
                .PrimaryKey(t => t.diagram_id);
            
            CreateTable(
                "dbo.UserAdmin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 50, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        Phone = c.String(nullable: false, maxLength: 20, unicode: false),
                        AdminType = c.Int(nullable: false),
                        LastLogin = c.DateTime(),
                        LastLogout = c.DateTime(),
                        AppZoneAccess = c.String(maxLength: 1000, unicode: false),
                        Password = c.String(nullable: false, maxLength: 500, unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        PanelType = c.Int(nullable: false),
                        UserId = c.String(maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Versioning",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PackageName = c.String(nullable: false, maxLength: 100, unicode: false),
                        VersionCode = c.Int(nullable: false),
                        VersionName = c.String(nullable: false, maxLength: 50, unicode: false),
                        Version = c.String(nullable: false, maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VoucherGenerator",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.String(nullable: false, maxLength: 20, unicode: false),
                        Nominal = c.Decimal(nullable: false, storeType: "money"),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        Quantity = c.Int(nullable: false),
                        VoucherType = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Voucher",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.String(nullable: false, maxLength: 50, unicode: false),
                        Nominal = c.Decimal(nullable: false, storeType: "money"),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        VoucherType = c.Int(nullable: false),
                        TrxId = c.Long(),
                        TrxType = c.String(maxLength: 10, unicode: false),
                        TrxUserId = c.Long(),
                        DateUsed = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NewsDetail", "NewsId", "dbo.News");
            DropForeignKey("dbo.DocumentAnnotate", "DocumentId", "dbo.Document");
            DropForeignKey("dbo.Document", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Stamp", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Message", "ToId", "dbo.Member");
            DropForeignKey("dbo.Message", "FromId", "dbo.Member");
            DropForeignKey("dbo.Message", "BroadcastMessageId", "dbo.BroadcastMessage");
            DropForeignKey("dbo.Member", "MemberTitleId", "dbo.MemberTitle");
            DropForeignKey("dbo.MemberSignHistory", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberPlanRequest", "SubscriptTypeId", "dbo.SubscriptType");
            DropForeignKey("dbo.MemberPlan", "SubscriptTypeId", "dbo.SubscriptType");
            DropForeignKey("dbo.MemberPlanExtra", "MemberPlanId", "dbo.MemberPlan");
            DropForeignKey("dbo.MemberPlan", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberPlanRequest", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberProject", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.MemberWorkflow", "WorkflowId", "dbo.Workflow");
            DropForeignKey("dbo.MemberRotation", "RotationId", "dbo.Rotation");
            DropForeignKey("dbo.Rotation", "WorkflowId", "dbo.Workflow");
            DropForeignKey("dbo.RotationMember", "WorkflowNodeId", "dbo.WorkflowNode");
            DropForeignKey("dbo.WorkflowNode", "WorkflowId", "dbo.Workflow");
            DropForeignKey("dbo.Workflow", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.Project", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.WorkflowNode", "SymbolId", "dbo.Symbol");
            DropForeignKey("dbo.WorkflowNodeLink", "WorkflowNodeToId", "dbo.WorkflowNode");
            DropForeignKey("dbo.WorkflowNodeLink", "WorkflowNodeId", "dbo.WorkflowNode");
            DropForeignKey("dbo.WorkflowNodeLink", "SymbolId", "dbo.Symbol");
            DropForeignKey("dbo.RotationNode", "WorkflowNodeId", "dbo.WorkflowNode");
            DropForeignKey("dbo.RotationNodeUpDoc", "RotationNodeId", "dbo.RotationNode");
            DropForeignKey("dbo.RotationNodeUpDoc", "DocumentUploadId", "dbo.DocumentUpload");
            DropForeignKey("dbo.RotationNodeRemark", "RotationNodeId", "dbo.RotationNode");
            DropForeignKey("dbo.RotationNodeDoc", "RotationNodeId", "dbo.RotationNode");
            DropForeignKey("dbo.RotationNodeDoc", "DocumentId", "dbo.Document");
            DropForeignKey("dbo.RotationNode", "SenderRotationNodeId", "dbo.RotationNode");
            DropForeignKey("dbo.RotationNode", "RotationId", "dbo.Rotation");
            DropForeignKey("dbo.RotationNode", "MemberId", "dbo.Member");
            DropForeignKey("dbo.WorkflowNode", "MemberId", "dbo.Member");
            DropForeignKey("dbo.RotationMember", "RotationId", "dbo.Rotation");
            DropForeignKey("dbo.RotationMember", "MemberId", "dbo.Member");
            DropForeignKey("dbo.Rotation", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberRotation", "MemberWorkflowId", "dbo.MemberWorkflow");
            DropForeignKey("dbo.MemberWorkflow", "MemberProjectId", "dbo.MemberProject");
            DropForeignKey("dbo.MemberProject", "MemberSubscribeId", "dbo.MemberSubscribe");
            DropForeignKey("dbo.MemberSubscribe", "MemberPermissionId", "dbo.MemberPermission");
            DropForeignKey("dbo.MemberSubscribe", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.MemberPermission", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberInvited", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberInvited", "InvitedId", "dbo.Member");
            DropForeignKey("dbo.MemberDepositTrx", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberDepositTransfer", "ToId", "dbo.Member");
            DropForeignKey("dbo.MemberDepositTransfer", "FromId", "dbo.Member");
            DropForeignKey("dbo.MemberAccount", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberAccount", "BankId", "dbo.Bank");
            DropForeignKey("dbo.CompanyBank", "PaymentMethodId", "dbo.PaymentMethod");
            DropForeignKey("dbo.MemberTopupPayment", "TopupDepositId", "dbo.MemberTopupDeposit");
            DropForeignKey("dbo.MemberTopupDeposit", "MemberId", "dbo.Member");
            DropForeignKey("dbo.MemberTopupPayment", "MemberAccountId", "dbo.MemberAccount");
            DropForeignKey("dbo.MemberTopupPayment", "CompanyBankId", "dbo.CompanyBank");
            DropForeignKey("dbo.CompanyBank", "BankId", "dbo.Bank");
            DropForeignKey("dbo.DocumentMember", "MemberId", "dbo.Member");
            DropForeignKey("dbo.DocumentMember", "DocumentId", "dbo.Document");
            DropForeignKey("dbo.Member", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.DocumentAnnotate", "AnnotateTypeId", "dbo.AnnotateType");
            DropIndex("dbo.NewsDetail", new[] { "NewsId" });
            DropIndex("dbo.Stamp", new[] { "CompanyId" });
            DropIndex("dbo.Message", new[] { "BroadcastMessageId" });
            DropIndex("dbo.Message", new[] { "ToId" });
            DropIndex("dbo.Message", new[] { "FromId" });
            DropIndex("dbo.MemberSignHistory", new[] { "MemberId" });
            DropIndex("dbo.MemberPlanExtra", new[] { "MemberPlanId" });
            DropIndex("dbo.MemberPlan", new[] { "SubscriptTypeId" });
            DropIndex("dbo.MemberPlan", new[] { "MemberId" });
            DropIndex("dbo.MemberPlanRequest", new[] { "SubscriptTypeId" });
            DropIndex("dbo.MemberPlanRequest", new[] { "MemberId" });
            DropIndex("dbo.Project", new[] { "CompanyId" });
            DropIndex("dbo.Workflow", new[] { "ProjectId" });
            DropIndex("dbo.WorkflowNodeLink", new[] { "SymbolId" });
            DropIndex("dbo.WorkflowNodeLink", new[] { "WorkflowNodeToId" });
            DropIndex("dbo.WorkflowNodeLink", new[] { "WorkflowNodeId" });
            DropIndex("dbo.RotationNodeUpDoc", new[] { "DocumentUploadId" });
            DropIndex("dbo.RotationNodeUpDoc", new[] { "RotationNodeId" });
            DropIndex("dbo.RotationNodeRemark", new[] { "RotationNodeId" });
            DropIndex("dbo.RotationNodeDoc", new[] { "DocumentId" });
            DropIndex("dbo.RotationNodeDoc", new[] { "RotationNodeId" });
            DropIndex("dbo.RotationNode", new[] { "MemberId" });
            DropIndex("dbo.RotationNode", new[] { "SenderRotationNodeId" });
            DropIndex("dbo.RotationNode", new[] { "WorkflowNodeId" });
            DropIndex("dbo.RotationNode", new[] { "RotationId" });
            DropIndex("dbo.WorkflowNode", new[] { "SymbolId" });
            DropIndex("dbo.WorkflowNode", new[] { "MemberId" });
            DropIndex("dbo.WorkflowNode", new[] { "WorkflowId" });
            DropIndex("dbo.RotationMember", new[] { "MemberId" });
            DropIndex("dbo.RotationMember", new[] { "WorkflowNodeId" });
            DropIndex("dbo.RotationMember", new[] { "RotationId" });
            DropIndex("dbo.Rotation", new[] { "MemberId" });
            DropIndex("dbo.Rotation", new[] { "WorkflowId" });
            DropIndex("dbo.MemberRotation", new[] { "RotationId" });
            DropIndex("dbo.MemberRotation", new[] { "MemberWorkflowId" });
            DropIndex("dbo.MemberWorkflow", new[] { "WorkflowId" });
            DropIndex("dbo.MemberWorkflow", new[] { "MemberProjectId" });
            DropIndex("dbo.MemberProject", new[] { "ProjectId" });
            DropIndex("dbo.MemberProject", new[] { "MemberSubscribeId" });
            DropIndex("dbo.MemberSubscribe", new[] { "CompanyId" });
            DropIndex("dbo.MemberSubscribe", new[] { "MemberPermissionId" });
            DropIndex("dbo.MemberPermission", new[] { "MemberId" });
            DropIndex("dbo.MemberInvited", new[] { "InvitedId" });
            DropIndex("dbo.MemberInvited", new[] { "MemberId" });
            DropIndex("dbo.MemberDepositTrx", new[] { "MemberId" });
            DropIndex("dbo.MemberDepositTransfer", new[] { "ToId" });
            DropIndex("dbo.MemberDepositTransfer", new[] { "FromId" });
            DropIndex("dbo.MemberTopupDeposit", new[] { "MemberId" });
            DropIndex("dbo.MemberTopupPayment", new[] { "MemberAccountId" });
            DropIndex("dbo.MemberTopupPayment", new[] { "CompanyBankId" });
            DropIndex("dbo.MemberTopupPayment", new[] { "TopupDepositId" });
            DropIndex("dbo.CompanyBank", new[] { "PaymentMethodId" });
            DropIndex("dbo.CompanyBank", new[] { "BankId" });
            DropIndex("dbo.MemberAccount", new[] { "BankId" });
            DropIndex("dbo.MemberAccount", new[] { "MemberId" });
            DropIndex("dbo.DocumentMember", new[] { "MemberId" });
            DropIndex("dbo.DocumentMember", new[] { "DocumentId" });
            DropIndex("dbo.Member", new[] { "CompanyId" });
            DropIndex("dbo.Member", new[] { "MemberTitleId" });
            DropIndex("dbo.Document", new[] { "CompanyId" });
            DropIndex("dbo.DocumentAnnotate", new[] { "AnnotateTypeId" });
            DropIndex("dbo.DocumentAnnotate", new[] { "DocumentId" });
            DropTable("dbo.Voucher");
            DropTable("dbo.VoucherGenerator");
            DropTable("dbo.Versioning");
            DropTable("dbo.UserAdmin");
            DropTable("dbo.sysdiagrams");
            DropTable("dbo.SubscriptExtraType");
            DropTable("dbo.StatusCode");
            DropTable("dbo.RotationNodeLog");
            DropTable("dbo.PodCast");
            DropTable("dbo.PaymentStatus");
            DropTable("dbo.NewsVideo");
            DropTable("dbo.NewsType");
            DropTable("dbo.NewsDetail");
            DropTable("dbo.News");
            DropTable("dbo.MemberType");
            DropTable("dbo.MemberHitLog");
            DropTable("dbo.MemberFolder");
            DropTable("dbo.FaspayPayment");
            DropTable("dbo.FaspayDebitStatus");
            DropTable("dbo.FaspayCreditStatus");
            DropTable("dbo.DrDriveType");
            DropTable("dbo.DrDrive");
            DropTable("dbo.ApplConfig");
            DropTable("dbo.Stamp");
            DropTable("dbo.BroadcastMessage");
            DropTable("dbo.Message");
            DropTable("dbo.MemberTitle");
            DropTable("dbo.MemberSignHistory");
            DropTable("dbo.MemberPlanExtra");
            DropTable("dbo.MemberPlan");
            DropTable("dbo.SubscriptType");
            DropTable("dbo.MemberPlanRequest");
            DropTable("dbo.Project");
            DropTable("dbo.Workflow");
            DropTable("dbo.WorkflowNodeLink");
            DropTable("dbo.Symbol");
            DropTable("dbo.DocumentUpload");
            DropTable("dbo.RotationNodeUpDoc");
            DropTable("dbo.RotationNodeRemark");
            DropTable("dbo.RotationNodeDoc");
            DropTable("dbo.RotationNode");
            DropTable("dbo.WorkflowNode");
            DropTable("dbo.RotationMember");
            DropTable("dbo.Rotation");
            DropTable("dbo.MemberRotation");
            DropTable("dbo.MemberWorkflow");
            DropTable("dbo.MemberProject");
            DropTable("dbo.MemberSubscribe");
            DropTable("dbo.MemberPermission");
            DropTable("dbo.MemberInvited");
            DropTable("dbo.MemberDepositTrx");
            DropTable("dbo.MemberDepositTransfer");
            DropTable("dbo.PaymentMethod");
            DropTable("dbo.MemberTopupDeposit");
            DropTable("dbo.MemberTopupPayment");
            DropTable("dbo.CompanyBank");
            DropTable("dbo.Bank");
            DropTable("dbo.MemberAccount");
            DropTable("dbo.DocumentMember");
            DropTable("dbo.Member");
            DropTable("dbo.Company");
            DropTable("dbo.Document");
            DropTable("dbo.DocumentAnnotate");
            DropTable("dbo.AnnotateType");
        }
    }
}
