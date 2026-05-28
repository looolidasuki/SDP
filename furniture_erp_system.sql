-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主机： 127.0.0.1
-- 生成日期： 2026-05-25 22:46:51
-- 服务器版本： 10.4.32-MariaDB
-- PHP 版本： 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 数据库： `furniture_erp_system`
--

-- --------------------------------------------------------

--
-- 表的结构 `contactperson`
--

CREATE TABLE `contactperson` (
  `contactPersonID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `customerID` bigint(20) NOT NULL COMMENT '关联客户',
  `contactPerson` varchar(100) DEFAULT NULL COMMENT '联系人姓名',
  `title` varchar(30) DEFAULT NULL COMMENT '称谓/职位',
  `phone` varchar(30) DEFAULT NULL COMMENT '电话',
  `email` varchar(255) DEFAULT NULL COMMENT '邮箱'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客户联系人明细表';

-- --------------------------------------------------------

--
-- 表的结构 `currency`
--

CREATE TABLE `currency` (
  `currencyID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `currencyCode` varchar(30) NOT NULL COMMENT '货币代码，如USD, HKD',
  `currencySymbol` varchar(5) NOT NULL COMMENT '货币符号',
  `rateToBase` decimal(10,2) NOT NULL COMMENT '当前币种对基准货币的汇率'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='货币汇率基础表';

--
-- 转存表中的数据 `currency`
--

INSERT INTO `currency` (`currencyID`, `currencyCode`, `currencySymbol`, `rateToBase`) VALUES
(1, 'HKD', '$', 1.00);

-- --------------------------------------------------------

--
-- 表的结构 `customer`
--

CREATE TABLE `customer` (
  `customerID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `customerName` varchar(255) DEFAULT NULL COMMENT '客户名称',
  `billingAddress` varchar(255) DEFAULT NULL COMMENT '账单地址',
  `paymentTerm` varchar(100) DEFAULT NULL COMMENT '付款条款',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp() COMMENT '创建时间',
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp() COMMENT '最后修改时间'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客户主表';

--
-- 转存表中的数据 `customer`
--

INSERT INTO `customer` (`customerID`, `customerName`, `billingAddress`, `paymentTerm`, `createDate`, `lastModifyDate`) VALUES
(1, 'ABC Clothing Ltd', NULL, NULL, '2026-05-25 17:16:09', '2026-05-25 17:16:09');

-- --------------------------------------------------------

--
-- 表的结构 `customerdeliveryaddress`
--

CREATE TABLE `customerdeliveryaddress` (
  `addressID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `customerID` bigint(20) NOT NULL COMMENT '关联客户',
  `deliveryAddress` varchar(255) DEFAULT NULL COMMENT '收货寄送地址',
  `contactPerson` varchar(100) DEFAULT NULL COMMENT '收货联系人',
  `phone` varchar(30) DEFAULT NULL COMMENT '收货电话',
  `email` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客户收货地址表';

-- --------------------------------------------------------

--
-- 表的结构 `deliverynote`
--

CREATE TABLE `deliverynote` (
  `deliveryNoteID` bigint(20) NOT NULL,
  `deliveryNoteCode` varchar(30) NOT NULL,
  `customerID` bigint(20) NOT NULL,
  `SalesOrderID` bigint(20) NOT NULL,
  `staffID` bigint(20) NOT NULL,
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp(),
  `WarehouseID` bigint(20) NOT NULL COMMENT '从哪一个出货物理仓出库',
  `shipMethod` varchar(30) NOT NULL COMMENT '发货运输方式',
  `trackingNumber` varchar(30) NOT NULL COMMENT '快递/物流追踪单号',
  `remark` varchar(255) DEFAULT NULL,
  `status` int(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='销售发货/出库流水单';

-- --------------------------------------------------------

--
-- 表的结构 `deliveryproductline`
--

CREATE TABLE `deliveryproductline` (
  `deliveryNoteID` bigint(20) NOT NULL,
  `productID` bigint(20) NOT NULL,
  `shipQuantity` int(10) NOT NULL COMMENT '本次包裹实际发货发出数量'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='发货单货品打包封装明细';

-- --------------------------------------------------------

--
-- 表的结构 `goodsreceivednote`
--

CREATE TABLE `goodsreceivednote` (
  `goodsReceivedNoteID` bigint(20) NOT NULL,
  `goodsReceivedNoteCode` varchar(30) NOT NULL,
  `supplierID` bigint(20) NOT NULL,
  `PurchaseOrderID` bigint(20) NOT NULL COMMENT '关联的采购单源头',
  `staffID` bigint(20) NOT NULL COMMENT '收货仓管验收员',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `status` int(10) NOT NULL,
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='原材料采购到货验收及入库单';

-- --------------------------------------------------------

--
-- 表的结构 `goodsreceivednoterawmaterialline`
--

CREATE TABLE `goodsreceivednoterawmaterialline` (
  `goodsReceivedNoteID` bigint(20) NOT NULL,
  `rawMaterialID` bigint(20) NOT NULL,
  `receivedQuantity` decimal(10,2) NOT NULL COMMENT '本次送达包裹清点合规后实际吃进库存的数量'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='采购到货入库原料明细账目';

-- --------------------------------------------------------

--
-- 表的结构 `invoice`
--

CREATE TABLE `invoice` (
  `invoiceID` bigint(20) NOT NULL,
  `invoiceCode` varchar(30) NOT NULL,
  `customerID` bigint(20) NOT NULL,
  `salesOrderID` bigint(20) NOT NULL,
  `staffID` bigint(20) NOT NULL,
  `invoiceType` int(10) NOT NULL COMMENT '类型：deposit(定金发票), normal(出货正规发票)',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp(),
  `remark` varchar(255) DEFAULT NULL,
  `status` int(10) NOT NULL COMMENT '开票对账状态'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='应收发票对账主表';

--
-- 转存表中的数据 `invoice`
--

INSERT INTO `invoice` (`invoiceID`, `invoiceCode`, `customerID`, `salesOrderID`, `staffID`, `invoiceType`, `createDate`, `lastModifyDate`, `remark`, `status`) VALUES
(1, 'INV-1', 1, 2, 1, 4, '2026-05-25 20:06:43', '2026-05-25 20:06:43', 'eqw', 1);

-- --------------------------------------------------------

--
-- 表的结构 `invoiceline`
--

CREATE TABLE `invoiceline` (
  `invoiceID` bigint(20) NOT NULL,
  `deliveryNoteID` bigint(20) NOT NULL COMMENT '多包裹分批出货时，开票需追溯对应的出库单',
  `productID` bigint(20) NOT NULL COMMENT '特殊：如果是deposit定金开票类型，此ID可以写入一个虚拟符号并在明细存负项冲平',
  `invoiceQuantity` int(10) NOT NULL COMMENT '本次开票计费数量',
  `amount` decimal(12,2) NOT NULL COMMENT '本次计费金额（包含负项冲减）'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='发票计费项目明细表（支持定金扣减逻辑）';

-- --------------------------------------------------------

--
-- 表的结构 `paymentvoucher`
--

CREATE TABLE `paymentvoucher` (
  `paymentVoucherID` bigint(20) NOT NULL,
  `paymentVoucherCode` varchar(30) NOT NULL,
  `supplierID` bigint(20) NOT NULL,
  `staffID` bigint(20) NOT NULL COMMENT '财务出纳审签经办人',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `paymentMethod` varchar(50) NOT NULL COMMENT '对公付汇渠道方式',
  `paymentMethodRef` varchar(100) NOT NULL COMMENT '银行付款水单参考号',
  `totalAmount` decimal(12,2) NOT NULL COMMENT '本次实际支付给该供应商的总汇款数',
  `remark` varchar(255) DEFAULT NULL,
  `status` int(10) NOT NULL COMMENT '应付款对账核销状态'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='应付上游供应商采购货款财务出账单';

-- --------------------------------------------------------

--
-- 表的结构 `paymentvoucherpurchaseorder`
--

CREATE TABLE `paymentvoucherpurchaseorder` (
  `paymentVoucherID` bigint(20) NOT NULL,
  `purchaseOrderID` bigint(20) NOT NULL,
  `type` int(10) NOT NULL COMMENT '应付对账阶段划分',
  `payAmount` decimal(12,2) NOT NULL COMMENT '本张水单里的款项中有多少额度被拿去核销了该张采购订单的欠款'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='财务应付账款实际冲账核销关联表';

-- --------------------------------------------------------

--
-- 表的结构 `product`
--

CREATE TABLE `product` (
  `productID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `productCode` varchar(30) NOT NULL COMMENT '模式 P-+category+sequenceNumber',
  `category` varchar(30) NOT NULL COMMENT '类别（如上衣、裤子）',
  `sequenceNumber` int(10) DEFAULT NULL COMMENT '序列编号',
  `styleNumber` varchar(30) NOT NULL COMMENT '衣服款号',
  `size` varchar(30) NOT NULL COMMENT '尺码',
  `color` varchar(30) NOT NULL COMMENT '颜色',
  `basePriceByCurrency` decimal(10,2) NOT NULL COMMENT '基本售价',
  `currencyID` bigint(20) NOT NULL COMMENT '定价币种',
  `staffID` bigint(20) NOT NULL COMMENT '录入员工',
  `unit` varchar(30) NOT NULL COMMENT '计量单位',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NULL DEFAULT NULL ON UPDATE current_timestamp(),
  `status` int(10) NOT NULL COMMENT '商品状态',
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='成品服饰SKU信息表';

--
-- 转存表中的数据 `product`
--

INSERT INTO `product` (`productID`, `productCode`, `category`, `sequenceNumber`, `styleNumber`, `size`, `color`, `basePriceByCurrency`, `currencyID`, `staffID`, `unit`, `createDate`, `lastModifyDate`, `status`, `remark`) VALUES
(2, 'P-Chair-2001', 'Chair', 2001, 'ERG-CHAIR-01', 'Standard High-Back', 'Matte Black', 1250.00, 1, 1, 'PCS', '2026-05-25 17:26:01', NULL, 1, 'Ergonomic office chair with adjustable mesh armrests and lumbar support.');

-- --------------------------------------------------------

--
-- 表的结构 `productimage`
--

CREATE TABLE `productimage` (
  `productID` bigint(20) NOT NULL COMMENT '一对一或多对一关联Product',
  `productImageUrl` varchar(255) DEFAULT NULL COMMENT '图片托管URL地址'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品图片附表';

-- --------------------------------------------------------

--
-- 表的结构 `productionorder`
--

CREATE TABLE `productionorder` (
  `productionOrderID` bigint(20) NOT NULL,
  `productionOrderCode` varchar(30) NOT NULL COMMENT '模式 PO-+ID',
  `salesOrderID` bigint(20) NOT NULL COMMENT '派生出此工单的销售单',
  `staffID` bigint(20) NOT NULL COMMENT '车间排产跟进员工',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `estFinishDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' COMMENT '预计完工交期',
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `status` int(10) NOT NULL COMMENT '车间生产状态控制',
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='车间服装生产工单表';

-- --------------------------------------------------------

--
-- 表的结构 `productionorderproductline`
--

CREATE TABLE `productionorderproductline` (
  `ProductionOrderID` bigint(20) NOT NULL,
  `productID` bigint(20) NOT NULL,
  `productionQty` int(10) NOT NULL COMMENT '计算逻辑：salesOrderProductLine.quantity - warehouseReservedQty'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='生产工单货品及数量明细';

-- --------------------------------------------------------

--
-- 表的结构 `productrawmaterialline`
--

CREATE TABLE `productrawmaterialline` (
  `productID` bigint(20) NOT NULL,
  `rawMaterialID` bigint(20) NOT NULL,
  `rawMaterialNeedQty` decimal(10,2) NOT NULL COMMENT '标准工艺下单件成品所需此原料的数量消耗定额',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='商品衣服物料清单配方（BOM表）';

-- --------------------------------------------------------

--
-- 表的结构 `purchaseorder`
--

CREATE TABLE `purchaseorder` (
  `purchaseOrderID` bigint(20) NOT NULL,
  `purchaseOrderCode` varchar(30) NOT NULL COMMENT '模式 PO-+ID',
  `supplierID` bigint(20) NOT NULL COMMENT '向哪家商户买料',
  `staffID` bigint(20) NOT NULL COMMENT '采购员',
  `relatedShortageReport` bigint(20) DEFAULT NULL COMMENT '可选追溯的系统缺货汇总单来源',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `requestDeliveryDate` date NOT NULL COMMENT '约束供货商到料交付的死线日期',
  `status` int(10) NOT NULL,
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='上游供应链原材料采购订单';

-- --------------------------------------------------------

--
-- 表的结构 `purchaseorderrawmaterialline`
--

CREATE TABLE `purchaseorderrawmaterialline` (
  `purchaseOrderID` bigint(20) NOT NULL,
  `rawMaterialID` bigint(20) NOT NULL,
  `price` decimal(10,2) NOT NULL COMMENT '采购议定单价',
  `orderQuantity` decimal(10,2) NOT NULL COMMENT '采购面料配件总数',
  `receivedQuantity` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '后期累计已完成收货清点的在途转实物入库数'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='供应链原材料采购订单明细行';

-- --------------------------------------------------------

--
-- 表的结构 `quotation`
--

CREATE TABLE `quotation` (
  `quotationID` bigint(20) NOT NULL,
  `quotationCode` varchar(30) NOT NULL COMMENT '模式 QT-+ID',
  `sequenceNumber` int(10) NOT NULL,
  `staffID` bigint(20) NOT NULL COMMENT '经办销售员工',
  `customerID` bigint(20) NOT NULL COMMENT '意向客户',
  `currencyID` bigint(20) NOT NULL COMMENT '报价币种',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `status` int(10) NOT NULL,
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='前期销售报价单';

-- --------------------------------------------------------

--
-- 表的结构 `quotationproductline`
--

CREATE TABLE `quotationproductline` (
  `quotationID` bigint(20) NOT NULL,
  `productID` bigint(20) NOT NULL,
  `price` decimal(10,2) NOT NULL COMMENT '报价单价',
  `quantity` decimal(10,2) NOT NULL COMMENT '意向数量',
  `discountAmount` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '折让金额'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='报价单成品明细';

-- --------------------------------------------------------

--
-- 表的结构 `rawmaterial`
--

CREATE TABLE `rawmaterial` (
  `rawMaterialID` bigint(20) NOT NULL,
  `rawMaterialCode` varchar(30) NOT NULL COMMENT '模式 RM-+category+sequenceNumber',
  `category` varchar(30) NOT NULL COMMENT '原料种类（如面料、纽扣、拉链）',
  `SequenceNumber` int(10) DEFAULT NULL,
  `size` varchar(30) NOT NULL,
  `color` varchar(30) NOT NULL,
  `minimumStockLevel` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '物料安全库存红线',
  `status` int(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='生产原材料SKU基础档案表';

-- --------------------------------------------------------

--
-- 表的结构 `rawmaterialrequestnote`
--

CREATE TABLE `rawmaterialrequestnote` (
  `rawMaterialRequestNoteID` bigint(20) NOT NULL,
  `rawMaterialRequestNoteCode` varchar(30) NOT NULL,
  `ProductionOrderID` bigint(20) NOT NULL COMMENT '车间关联的源头排产生产订单',
  `staffID` bigint(20) NOT NULL COMMENT '申领发起员工',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `requestDate` date NOT NULL COMMENT '期望要求的到料领料日期',
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='车间向仓储/采购部提报的物料领料申领及请购单';

-- --------------------------------------------------------

--
-- 表的结构 `rawmaterialrequestnoterawmaterial_line`
--

CREATE TABLE `rawmaterialrequestnoterawmaterial_line` (
  `rawMaterialRequestNoteID` bigint(20) NOT NULL,
  `productID` bigint(20) NOT NULL COMMENT '要切片对应哪一个成品所需制造的配给',
  `rawMaterialID` bigint(20) NOT NULL COMMENT '具体申领的原材料',
  `rawMaterialRequestQuantity` decimal(10,2) NOT NULL COMMENT '本次申请流转的数量'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='领料请购单精确物料行明细';

-- --------------------------------------------------------

--
-- 表的结构 `rawmaterialshortagereportline`
--

CREATE TABLE `rawmaterialshortagereportline` (
  `shortageReportID` bigint(20) NOT NULL,
  `rawMaterialID` bigint(20) NOT NULL,
  `WarehousewarehouseID` bigint(20) NOT NULL,
  `totalShortageQuantity` decimal(10,2) NOT NULL COMMENT '通过盘点自动轧出的真实缺货数量'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='物料缺货清单明细';

-- --------------------------------------------------------

--
-- 表的结构 `rawmaterialsupplier`
--

CREATE TABLE `rawmaterialsupplier` (
  `rawMaterialID` bigint(20) NOT NULL,
  `supplierID` bigint(20) NOT NULL,
  `supplierStyleNumber` varchar(50) DEFAULT NULL COMMENT '供应商在自己厂内对应的物料款号',
  `basePrice` decimal(10,2) NOT NULL COMMENT '供货报价',
  `currencyID` bigint(20) NOT NULL COMMENT '供货结算币种',
  `unit` varchar(30) NOT NULL COMMENT '计量供应单位',
  `minimumOrderQuantity` int(10) NOT NULL DEFAULT 1 COMMENT '最小起订量限制',
  `quoteDate` date DEFAULT NULL COMMENT '报价生效起始日',
  `lastModify` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `status` int(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='供应商原材料价格与起订量名录';

-- --------------------------------------------------------

--
-- 表的结构 `rawmaterialwarehouse`
--

CREATE TABLE `rawmaterialwarehouse` (
  `rawMaterialID` bigint(20) NOT NULL,
  `warehouseID` bigint(20) NOT NULL,
  `physicalQuantity` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '面料辅料实际物理库存',
  `reservedQuantity` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '已被排产锁定消耗的原料数',
  `purchasedQuantity` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '已下采购单等在途的原材料数'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='原材料仓储动态库存对账表';

-- --------------------------------------------------------

--
-- 表的结构 `receiptvoucher`
--

CREATE TABLE `receiptvoucher` (
  `receiptVoucherID` bigint(20) NOT NULL,
  `receiptVoucherCode` varchar(30) NOT NULL,
  `cusomerID` bigint(20) NOT NULL COMMENT '原图拼写为 cusomerID 保持一致',
  `staffID` bigint(20) NOT NULL,
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `paymentMethod` varchar(30) NOT NULL COMMENT '付款通道',
  `paymentMethodRef` varchar(30) NOT NULL COMMENT '支付流水参考凭证号',
  `paymentAmount` decimal(10,2) NOT NULL COMMENT '实收总金额',
  `currencyID` bigint(20) NOT NULL COMMENT '实收币种',
  `paymentReceivedDate` date NOT NULL COMMENT '实际到账日期',
  `status` int(10) NOT NULL,
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客户财务收款进账流水单';

-- --------------------------------------------------------

--
-- 表的结构 `receiptvoucherinvoice`
--

CREATE TABLE `receiptvoucherinvoice` (
  `receiptVoucherID` bigint(20) NOT NULL,
  `invoiceID` bigint(20) NOT NULL,
  `receivedAmount` decimal(10,2) NOT NULL COMMENT '这笔收款核销拆分拨给该发票的额度。SUM(receivedAmount) 必须等于 receiptVoucher.paymentAmount',
  `type` int(10) NOT NULL COMMENT '核销阶段类型：deposit, partial, final, exchangeLoss(汇损结转)'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='财务应收实收关联核销表';

-- --------------------------------------------------------

--
-- 表的结构 `refundrequest`
--

CREATE TABLE `refundrequest` (
  `refundRequestID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `refundRequestCode` varchar(30) NOT NULL COMMENT '固定模式 RF-+ID',
  `staffID` bigint(20) NOT NULL COMMENT '经办员工',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `ReceiptVoucherID` bigint(20) DEFAULT NULL COMMENT '关联收款凭证（可选）',
  `InvoiceID` bigint(20) DEFAULT NULL COMMENT '关联发票（可选）',
  `refundAmount` decimal(19,2) NOT NULL COMMENT '退款金额',
  `refundMethod` tinyint(4) NOT NULL COMMENT '退款方式（固定选择，由字典表控制，1:bank transfer, 2:FPS, 3:cheque等）',
  `refundRef` varchar(100) DEFAULT NULL COMMENT '员工输入的支付网关交易参考号',
  `refundReason` varchar(100) NOT NULL COMMENT '退款原因（固定选择，如 damage, wrong shipment等）',
  `status` int(10) NOT NULL COMMENT '单据状态',
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='退款申请流水表';

--
-- 转存表中的数据 `refundrequest`
--

INSERT INTO `refundrequest` (`refundRequestID`, `refundRequestCode`, `staffID`, `createDate`, `ReceiptVoucherID`, `InvoiceID`, `refundAmount`, `refundMethod`, `refundRef`, `refundReason`, `status`, `lastModifyDate`, `remark`) VALUES
(2, 'RF-2026052601', 1, '2026-05-25 16:25:50', 0, 0, 250.50, 1, 'REF-XYZ123', 'damage', 1, '2026-05-25 16:25:50', 'Test refund request');

-- --------------------------------------------------------

--
-- 表的结构 `salesorder`
--

CREATE TABLE `salesorder` (
  `salesOrderID` bigint(20) NOT NULL,
  `salesOrderCode` varchar(30) NOT NULL COMMENT '模式 SO-+ID',
  `customerID` bigint(20) NOT NULL,
  `staffID` bigint(20) NOT NULL,
  `currencyCurrencyID` bigint(20) NOT NULL COMMENT '交易币种',
  `deliveryAddress` varchar(255) NOT NULL COMMENT '原图标记为date属笔误，修正为字符串存放发货地址',
  `createDate` timestamp NOT NULL DEFAULT current_timestamp(),
  `lastModifyDate` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `discountType` varchar(30) DEFAULT NULL COMMENT '折扣类型分类',
  `discount` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '总单减免折扣',
  `status` int(10) NOT NULL COMMENT '状态机控制：草稿、已锁定、生产中、发货完成等',
  `remark` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='核心销售订单表';

--
-- 转存表中的数据 `salesorder`
--

INSERT INTO `salesorder` (`salesOrderID`, `salesOrderCode`, `customerID`, `staffID`, `currencyCurrencyID`, `deliveryAddress`, `createDate`, `lastModifyDate`, `discountType`, `discount`, `status`, `remark`) VALUES
(1, 'SO-2026052601', 1, 1, 1, 'Flat B, 5/F, Industrial Building, Kwun Tong, Hong Kong', '2026-05-25 17:17:02', '2026-05-25 17:17:02', 'Percentage', 500.00, 1, 'First test sales order for system integration.'),
(2, 'SO-2', 1, 1, 1, 'TBD', '2026-05-25 20:05:57', '2026-05-25 20:05:57', NULL, 0.00, 0, NULL);

-- --------------------------------------------------------

--
-- 表的结构 `salesorderproductline`
--

CREATE TABLE `salesorderproductline` (
  `salesOrderID` bigint(20) NOT NULL,
  `productID` bigint(20) NOT NULL,
  `price` decimal(10,2) NOT NULL COMMENT '实际销售单价',
  `orderQuantity` decimal(10,2) NOT NULL COMMENT '定购总数量',
  `discountAmount` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '单品折让',
  `warehouseReservedQty` int(10) NOT NULL DEFAULT 0 COMMENT '已从实体仓库占用的预留配额',
  `shippedQuantity` int(10) NOT NULL DEFAULT 0 COMMENT '已发货交付累计数',
  `invoicedQuantity` int(10) NOT NULL DEFAULT 0 COMMENT '已开具发票累计数'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='销售订单商品货品细项';

-- --------------------------------------------------------

--
-- 表的结构 `shortagereport`
--

CREATE TABLE `shortagereport` (
  `shortageReportID` bigint(20) NOT NULL,
  `shortageReportCode` varchar(30) NOT NULL COMMENT '模式 SR-+date+sequenceNumber',
  `date` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp() COMMENT '报告引发或计算生成的基准结算时间',
  `sequenceNumber` int(10) NOT NULL,
  `createDate` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='系统自动扫描或手动生成的原料缺货汇总报告';

-- --------------------------------------------------------

--
-- 表的结构 `staff`
--

CREATE TABLE `staff` (
  `staffID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `username` varchar(30) NOT NULL COMMENT '登录用户名',
  `password` varchar(255) NOT NULL COMMENT '加密密码',
  `title` varchar(30) NOT NULL COMMENT '职位职称',
  `department` varchar(30) NOT NULL COMMENT '所属部门',
  `firstName` varchar(30) NOT NULL,
  `lastName` varchar(30) NOT NULL,
  `employDate` date NOT NULL COMMENT '入职日期',
  `phone` varchar(30) NOT NULL,
  `email` varchar(255) NOT NULL,
  `status` int(10) DEFAULT NULL COMMENT '员工状态'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='员工与用户主表';

--
-- 转存表中的数据 `staff`
--

INSERT INTO `staff` (`staffID`, `username`, `password`, `title`, `department`, `firstName`, `lastName`, `employDate`, `phone`, `email`, `status`) VALUES
(1, 'admin', '123456', 'Manager', 'Finance', 'John', 'Doe', '2026-05-26', '12345678', 'john.doe@erp.com', 1);

-- --------------------------------------------------------

--
-- 表的结构 `supplier`
--

CREATE TABLE `supplier` (
  `supplierID` bigint(20) NOT NULL,
  `supplierName` varchar(255) NOT NULL,
  `billingAddress` varchar(255) DEFAULT NULL,
  `contactPerson` varchar(100) DEFAULT NULL,
  `phone` varchar(30) DEFAULT NULL,
  `email` varchar(255) DEFAULT NULL,
  `paymentTerm` varchar(100) DEFAULT NULL COMMENT '与供应商约定的结算账期条件',
  `bankAccount` varchar(100) DEFAULT NULL COMMENT '供应商收汇对公账户',
  `status` int(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='面料及配件上游供应商主体表';

-- --------------------------------------------------------

--
-- 表的结构 `systemdictionary`
--

CREATE TABLE `systemdictionary` (
  `dictionaryID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `category` varchar(60) NOT NULL COMMENT '字典类别，例如 refundMethod, refundReason',
  `codeValue` tinyint(4) NOT NULL COMMENT '状态码值，如 1, 2, 3',
  `displayNameEnglish` varchar(50) NOT NULL COMMENT '英文显示名',
  `sortOrder` int(10) NOT NULL DEFAULT 0 COMMENT '排序权值'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='数据字典配置表';

--
-- 转存表中的数据 `systemdictionary`
--

INSERT INTO `systemdictionary` (`dictionaryID`, `category`, `codeValue`, `displayNameEnglish`, `sortOrder`) VALUES
(1, 'PURCHASE_ORDER_STATUS', 0, 'Draft', 1),
(2, 'PURCHASE_ORDER_STATUS', 1, 'Pending Approval', 2),
(3, 'PURCHASE_ORDER_STATUS', 2, 'Approved', 3),
(4, 'PURCHASE_ORDER_STATUS', 3, 'Rejected', 4),
(5, 'PURCHASE_ORDER_STATUS', 4, 'Ordered', 5),
(6, 'PURCHASE_ORDER_STATUS', 5, 'Receiving', 6),
(7, 'PURCHASE_ORDER_STATUS', 6, 'Completed', 7),
(8, 'PURCHASE_ORDER_STATUS', 7, 'Cancelled', 8),
(9, 'SUPPLIER_STATUS', 1, 'Active', 1),
(10, 'SUPPLIER_STATUS', 0, 'Inactive', 2),
(11, 'PAYMENT_TERM', 1, 'Cash', 1),
(12, 'PAYMENT_TERM', 2, '30 Days', 2),
(13, 'PAYMENT_TERM', 3, '60 Days', 3),
(14, 'PAYMENT_TERM', 4, '90 Days', 4),
(15, 'SALES_ORDER_STATUS', 0, 'Draft', 1),
(16, 'SALES_ORDER_STATUS', 1, 'Confirmed', 2),
(17, 'SALES_ORDER_STATUS', 2, 'Processing', 3),
(18, 'SALES_ORDER_STATUS', 3, 'Shipped', 4),
(19, 'SALES_ORDER_STATUS', 4, 'Completed', 5),
(20, 'SALES_ORDER_STATUS', 5, 'Cancelled', 6),
(21, 'DISCOUNT_TYPE', 1, 'Percentage', 1),
(22, 'DISCOUNT_TYPE', 2, 'Fixed Amount', 2),
(23, 'STAFF_STATUS', 1, 'Active', 1),
(24, 'STAFF_STATUS', 0, 'Inactive', 2),
(25, 'DEPARTMENT', 1, 'Sales', 1),
(26, 'DEPARTMENT', 2, 'Purchasing', 2),
(27, 'DEPARTMENT', 3, 'Finance', 3),
(28, 'DEPARTMENT', 4, 'Production', 4),
(29, 'DEPARTMENT', 5, 'Warehouse', 5),
(30, 'STAFF_TITLE', 1, 'Manager', 1),
(31, 'STAFF_TITLE', 2, 'Officer', 2),
(32, 'STAFF_TITLE', 3, 'Clerk', 3),
(33, 'STAFF_TITLE', 4, 'Director', 4),
(34, 'REFUND_METHOD', 1, 'Bank Transfer', 1),
(35, 'REFUND_METHOD', 2, 'FPS', 2),
(36, 'REFUND_METHOD', 3, 'Cheque', 3),
(37, 'REFUND_METHOD', 4, 'TT', 4),
(38, 'REFUND_METHOD', 5, 'PayPal', 5),
(39, 'REFUND_METHOD', 6, 'Amazon Pay', 6),
(40, 'REFUND_METHOD', 7, 'Taobao Pay', 7),
(41, 'REFUND_REASON', 1, 'Damage', 1),
(42, 'REFUND_REASON', 2, 'Wrong Shipment', 2),
(43, 'REFUND_REASON', 3, 'Sizing Issue', 3),
(44, 'REFUND_REASON', 4, 'Order Cancelled', 4),
(45, 'REFUND_REASON', 5, 'Customer Dissatisfaction', 5),
(46, 'PRODUCT_STATUS', 1, 'Active', 1),
(47, 'PRODUCT_STATUS', 0, 'Inactive', 2),
(48, 'PRODUCT_STATUS', 2, 'Out of Stock', 3),
(49, 'PRODUCT_STATUS', 3, 'Discontinued', 4),
(50, 'PRODUCTION_STATUS', 0, 'Pending Scheduling', 1),
(51, 'PRODUCTION_STATUS', 1, 'In Progress', 2),
(52, 'PRODUCTION_STATUS', 2, 'Quality Checking', 3),
(53, 'PRODUCTION_STATUS', 3, 'Completed', 4),
(54, 'PRODUCTION_STATUS', 4, 'Paused', 5),
(55, 'DELIVERY_STATUS', 0, 'Preparing', 1),
(56, 'DELIVERY_STATUS', 1, 'Packed', 2),
(57, 'DELIVERY_STATUS', 2, 'In Transit', 3),
(58, 'DELIVERY_STATUS', 3, 'Delivered', 4),
(59, 'DELIVERY_STATUS', 4, 'Returned', 5),
(60, 'INVOICE_STATUS', 0, 'Unpaid', 1),
(61, 'INVOICE_STATUS', 1, 'Partially Paid', 2),
(62, 'INVOICE_STATUS', 2, 'Fully Paid', 3),
(63, 'INVOICE_STATUS', 3, 'Overdue', 4),
(64, 'INVOICE_STATUS', 4, 'Voided', 5),
(65, 'RECEIPT_VOUCHER_STATUS', 0, 'Pending Verification', 1),
(66, 'RECEIPT_VOUCHER_STATUS', 1, 'Verified', 2),
(67, 'RECEIPT_VOUCHER_STATUS', 2, 'Rejected', 3),
(68, 'FINANCIAL_CLEARING_TYPE', 1, 'Deposit', 1),
(69, 'FINANCIAL_CLEARING_TYPE', 2, 'Partial Payment', 2),
(70, 'FINANCIAL_CLEARING_TYPE', 3, 'Final Payment', 3),
(71, 'FINANCIAL_CLEARING_TYPE', 4, 'Exchange Loss', 4),
(72, 'RAW_MATERIAL_STATUS', 1, 'Active', 1),
(73, 'RAW_MATERIAL_STATUS', 0, 'Inactive', 2),
(74, 'RAW_MATERIAL_STATUS', 2, 'Below Safety Stock', 3);

-- --------------------------------------------------------

--
-- 表的结构 `systemdictionary_refundrequest`
--

CREATE TABLE `systemdictionary_refundrequest` (
  `SystemDictionarydictionaryID` bigint(20) NOT NULL,
  `SystemDictionarycategory` varchar(60) NOT NULL,
  `SystemDictionarycodeValue` tinyint(4) NOT NULL,
  `RefundRequestrefundRequestID` bigint(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='字典与退款申请桥接映射表';

-- --------------------------------------------------------

--
-- 表的结构 `warehouse`
--

CREATE TABLE `warehouse` (
  `warehouseID` bigint(20) NOT NULL COMMENT '自增唯一标识',
  `warehouseName` varchar(30) NOT NULL COMMENT '仓库名称',
  `warehouseAddress` varchar(255) NOT NULL COMMENT '仓库地址'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='多仓储区域定义表';

-- --------------------------------------------------------

--
-- 表的结构 `warehouseproduct`
--

CREATE TABLE `warehouseproduct` (
  `warehouseID` bigint(20) NOT NULL,
  `productID` bigint(20) NOT NULL,
  `physicalQuantity` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '实物库存',
  `reservedQuantity` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '被销售单预留锁定的库存',
  `purchasedQuantity` decimal(10,2) NOT NULL DEFAULT 0.00 COMMENT '已下单采购但尚未入库的数量'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='仓库成品物理与逻辑库存对账表';

--
-- 转储表的索引
--

--
-- 表的索引 `contactperson`
--
ALTER TABLE `contactperson`
  ADD PRIMARY KEY (`contactPersonID`),
  ADD KEY `fk_contact_customer` (`customerID`);

--
-- 表的索引 `currency`
--
ALTER TABLE `currency`
  ADD PRIMARY KEY (`currencyID`),
  ADD UNIQUE KEY `currencyCode` (`currencyCode`);

--
-- 表的索引 `customer`
--
ALTER TABLE `customer`
  ADD PRIMARY KEY (`customerID`);

--
-- 表的索引 `customerdeliveryaddress`
--
ALTER TABLE `customerdeliveryaddress`
  ADD PRIMARY KEY (`addressID`),
  ADD KEY `fk_address_customer` (`customerID`);

--
-- 表的索引 `deliverynote`
--
ALTER TABLE `deliverynote`
  ADD PRIMARY KEY (`deliveryNoteID`),
  ADD UNIQUE KEY `deliveryNoteCode` (`deliveryNoteCode`),
  ADD KEY `fk_dn_customer` (`customerID`),
  ADD KEY `fk_dn_so` (`SalesOrderID`),
  ADD KEY `fk_dn_staff` (`staffID`),
  ADD KEY `fk_dn_warehouse` (`WarehouseID`);

--
-- 表的索引 `deliveryproductline`
--
ALTER TABLE `deliveryproductline`
  ADD PRIMARY KEY (`deliveryNoteID`,`productID`),
  ADD KEY `fk_dline_product` (`productID`);

--
-- 表的索引 `goodsreceivednote`
--
ALTER TABLE `goodsreceivednote`
  ADD PRIMARY KEY (`goodsReceivedNoteID`),
  ADD UNIQUE KEY `goodsReceivedNoteCode` (`goodsReceivedNoteCode`),
  ADD KEY `fk_grn_supplier` (`supplierID`),
  ADD KEY `fk_grn_pur` (`PurchaseOrderID`),
  ADD KEY `fk_grn_staff` (`staffID`);

--
-- 表的索引 `goodsreceivednoterawmaterialline`
--
ALTER TABLE `goodsreceivednoterawmaterialline`
  ADD PRIMARY KEY (`goodsReceivedNoteID`,`rawMaterialID`),
  ADD KEY `fk_grnline_raw` (`rawMaterialID`);

--
-- 表的索引 `invoice`
--
ALTER TABLE `invoice`
  ADD PRIMARY KEY (`invoiceID`),
  ADD UNIQUE KEY `invoiceCode` (`invoiceCode`),
  ADD KEY `fk_inv_customer` (`customerID`),
  ADD KEY `fk_inv_so` (`salesOrderID`),
  ADD KEY `fk_inv_staff` (`staffID`);

--
-- 表的索引 `invoiceline`
--
ALTER TABLE `invoiceline`
  ADD PRIMARY KEY (`invoiceID`,`deliveryNoteID`,`productID`),
  ADD KEY `fk_invline_product` (`productID`);

--
-- 表的索引 `paymentvoucher`
--
ALTER TABLE `paymentvoucher`
  ADD PRIMARY KEY (`paymentVoucherID`),
  ADD UNIQUE KEY `paymentVoucherCode` (`paymentVoucherCode`),
  ADD KEY `fk_pv_supplier` (`supplierID`),
  ADD KEY `fk_pv_staff` (`staffID`);

--
-- 表的索引 `paymentvoucherpurchaseorder`
--
ALTER TABLE `paymentvoucherpurchaseorder`
  ADD PRIMARY KEY (`paymentVoucherID`,`purchaseOrderID`),
  ADD KEY `fk_pvpo_po` (`purchaseOrderID`);

--
-- 表的索引 `product`
--
ALTER TABLE `product`
  ADD PRIMARY KEY (`productID`),
  ADD UNIQUE KEY `productCode` (`productCode`),
  ADD KEY `fk_product_currency` (`currencyID`),
  ADD KEY `fk_product_staff` (`staffID`);

--
-- 表的索引 `productimage`
--
ALTER TABLE `productimage`
  ADD PRIMARY KEY (`productID`);

--
-- 表的索引 `productionorder`
--
ALTER TABLE `productionorder`
  ADD PRIMARY KEY (`productionOrderID`),
  ADD UNIQUE KEY `productionOrderCode` (`productionOrderCode`),
  ADD KEY `fk_po_so` (`salesOrderID`),
  ADD KEY `fk_po_staff` (`staffID`);

--
-- 表的索引 `productionorderproductline`
--
ALTER TABLE `productionorderproductline`
  ADD PRIMARY KEY (`ProductionOrderID`,`productID`),
  ADD KEY `fk_poline_product` (`productID`);

--
-- 表的索引 `productrawmaterialline`
--
ALTER TABLE `productrawmaterialline`
  ADD PRIMARY KEY (`productID`,`rawMaterialID`),
  ADD KEY `fk_bom_raw` (`rawMaterialID`);

--
-- 表的索引 `purchaseorder`
--
ALTER TABLE `purchaseorder`
  ADD PRIMARY KEY (`purchaseOrderID`),
  ADD UNIQUE KEY `purchaseOrderCode` (`purchaseOrderCode`),
  ADD KEY `fk_pur_supplier` (`supplierID`),
  ADD KEY `fk_pur_staff` (`staffID`);

--
-- 表的索引 `purchaseorderrawmaterialline`
--
ALTER TABLE `purchaseorderrawmaterialline`
  ADD PRIMARY KEY (`purchaseOrderID`,`rawMaterialID`),
  ADD KEY `fk_purline_raw` (`rawMaterialID`);

--
-- 表的索引 `quotation`
--
ALTER TABLE `quotation`
  ADD PRIMARY KEY (`quotationID`),
  ADD UNIQUE KEY `quotationCode` (`quotationCode`),
  ADD KEY `fk_quote_staff` (`staffID`),
  ADD KEY `fk_quote_customer` (`customerID`),
  ADD KEY `fk_quote_currency` (`currencyID`);

--
-- 表的索引 `quotationproductline`
--
ALTER TABLE `quotationproductline`
  ADD PRIMARY KEY (`quotationID`,`productID`),
  ADD KEY `fk_qline_product` (`productID`);

--
-- 表的索引 `rawmaterial`
--
ALTER TABLE `rawmaterial`
  ADD PRIMARY KEY (`rawMaterialID`),
  ADD UNIQUE KEY `rawMaterialCode` (`rawMaterialCode`);

--
-- 表的索引 `rawmaterialrequestnote`
--
ALTER TABLE `rawmaterialrequestnote`
  ADD PRIMARY KEY (`rawMaterialRequestNoteID`),
  ADD UNIQUE KEY `rawMaterialRequestNoteCode` (`rawMaterialRequestNoteCode`),
  ADD KEY `fk_rmreq_po` (`ProductionOrderID`),
  ADD KEY `fk_rmreq_staff` (`staffID`);

--
-- 表的索引 `rawmaterialrequestnoterawmaterial_line`
--
ALTER TABLE `rawmaterialrequestnoterawmaterial_line`
  ADD PRIMARY KEY (`rawMaterialRequestNoteID`,`productID`,`rawMaterialID`),
  ADD KEY `fk_rmreqline_raw` (`rawMaterialID`);

--
-- 表的索引 `rawmaterialshortagereportline`
--
ALTER TABLE `rawmaterialshortagereportline`
  ADD PRIMARY KEY (`shortageReportID`,`rawMaterialID`,`WarehousewarehouseID`),
  ADD KEY `fk_srline_raw` (`rawMaterialID`),
  ADD KEY `fk_srline_wh` (`WarehousewarehouseID`);

--
-- 表的索引 `rawmaterialsupplier`
--
ALTER TABLE `rawmaterialsupplier`
  ADD PRIMARY KEY (`rawMaterialID`,`supplierID`),
  ADD KEY `fk_rms_sup` (`supplierID`);

--
-- 表的索引 `rawmaterialwarehouse`
--
ALTER TABLE `rawmaterialwarehouse`
  ADD PRIMARY KEY (`rawMaterialID`,`warehouseID`),
  ADD KEY `fk_rmw_warehouse` (`warehouseID`);

--
-- 表的索引 `receiptvoucher`
--
ALTER TABLE `receiptvoucher`
  ADD PRIMARY KEY (`receiptVoucherID`),
  ADD UNIQUE KEY `receiptVoucherCode` (`receiptVoucherCode`),
  ADD KEY `fk_rv_customer` (`cusomerID`),
  ADD KEY `fk_rv_staff` (`staffID`),
  ADD KEY `fk_rv_currency` (`currencyID`);

--
-- 表的索引 `receiptvoucherinvoice`
--
ALTER TABLE `receiptvoucherinvoice`
  ADD PRIMARY KEY (`receiptVoucherID`,`invoiceID`),
  ADD KEY `fk_rvi_inv` (`invoiceID`);

--
-- 表的索引 `refundrequest`
--
ALTER TABLE `refundrequest`
  ADD PRIMARY KEY (`refundRequestID`),
  ADD UNIQUE KEY `refundRequestCode` (`refundRequestCode`),
  ADD KEY `fk_refund_staff` (`staffID`);

--
-- 表的索引 `salesorder`
--
ALTER TABLE `salesorder`
  ADD PRIMARY KEY (`salesOrderID`),
  ADD UNIQUE KEY `salesOrderCode` (`salesOrderCode`),
  ADD KEY `fk_so_customer` (`customerID`),
  ADD KEY `fk_so_staff` (`staffID`),
  ADD KEY `fk_so_currency` (`currencyCurrencyID`);

--
-- 表的索引 `salesorderproductline`
--
ALTER TABLE `salesorderproductline`
  ADD PRIMARY KEY (`salesOrderID`,`productID`),
  ADD KEY `fk_soline_product` (`productID`);

--
-- 表的索引 `shortagereport`
--
ALTER TABLE `shortagereport`
  ADD PRIMARY KEY (`shortageReportID`),
  ADD UNIQUE KEY `shortageReportCode` (`shortageReportCode`);

--
-- 表的索引 `staff`
--
ALTER TABLE `staff`
  ADD PRIMARY KEY (`staffID`),
  ADD UNIQUE KEY `username` (`username`);

--
-- 表的索引 `supplier`
--
ALTER TABLE `supplier`
  ADD PRIMARY KEY (`supplierID`);

--
-- 表的索引 `systemdictionary`
--
ALTER TABLE `systemdictionary`
  ADD PRIMARY KEY (`dictionaryID`),
  ADD UNIQUE KEY `uk_category_value` (`category`,`codeValue`);

--
-- 表的索引 `systemdictionary_refundrequest`
--
ALTER TABLE `systemdictionary_refundrequest`
  ADD PRIMARY KEY (`SystemDictionarydictionaryID`,`RefundRequestrefundRequestID`),
  ADD KEY `fk_bridge_refund` (`RefundRequestrefundRequestID`);

--
-- 表的索引 `warehouse`
--
ALTER TABLE `warehouse`
  ADD PRIMARY KEY (`warehouseID`);

--
-- 表的索引 `warehouseproduct`
--
ALTER TABLE `warehouseproduct`
  ADD PRIMARY KEY (`warehouseID`,`productID`),
  ADD KEY `fk_wp_product` (`productID`);

--
-- 在导出的表使用AUTO_INCREMENT
--

--
-- 使用表AUTO_INCREMENT `contactperson`
--
ALTER TABLE `contactperson`
  MODIFY `contactPersonID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识';

--
-- 使用表AUTO_INCREMENT `currency`
--
ALTER TABLE `currency`
  MODIFY `currencyID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识', AUTO_INCREMENT=2;

--
-- 使用表AUTO_INCREMENT `customer`
--
ALTER TABLE `customer`
  MODIFY `customerID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识', AUTO_INCREMENT=2;

--
-- 使用表AUTO_INCREMENT `customerdeliveryaddress`
--
ALTER TABLE `customerdeliveryaddress`
  MODIFY `addressID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识';

--
-- 使用表AUTO_INCREMENT `deliverynote`
--
ALTER TABLE `deliverynote`
  MODIFY `deliveryNoteID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `goodsreceivednote`
--
ALTER TABLE `goodsreceivednote`
  MODIFY `goodsReceivedNoteID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `invoice`
--
ALTER TABLE `invoice`
  MODIFY `invoiceID` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- 使用表AUTO_INCREMENT `paymentvoucher`
--
ALTER TABLE `paymentvoucher`
  MODIFY `paymentVoucherID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `product`
--
ALTER TABLE `product`
  MODIFY `productID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识', AUTO_INCREMENT=3;

--
-- 使用表AUTO_INCREMENT `productionorder`
--
ALTER TABLE `productionorder`
  MODIFY `productionOrderID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `purchaseorder`
--
ALTER TABLE `purchaseorder`
  MODIFY `purchaseOrderID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `quotation`
--
ALTER TABLE `quotation`
  MODIFY `quotationID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `rawmaterial`
--
ALTER TABLE `rawmaterial`
  MODIFY `rawMaterialID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `rawmaterialrequestnote`
--
ALTER TABLE `rawmaterialrequestnote`
  MODIFY `rawMaterialRequestNoteID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `receiptvoucher`
--
ALTER TABLE `receiptvoucher`
  MODIFY `receiptVoucherID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `refundrequest`
--
ALTER TABLE `refundrequest`
  MODIFY `refundRequestID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识', AUTO_INCREMENT=3;

--
-- 使用表AUTO_INCREMENT `salesorder`
--
ALTER TABLE `salesorder`
  MODIFY `salesOrderID` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- 使用表AUTO_INCREMENT `shortagereport`
--
ALTER TABLE `shortagereport`
  MODIFY `shortageReportID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `staff`
--
ALTER TABLE `staff`
  MODIFY `staffID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识', AUTO_INCREMENT=2;

--
-- 使用表AUTO_INCREMENT `supplier`
--
ALTER TABLE `supplier`
  MODIFY `supplierID` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- 使用表AUTO_INCREMENT `systemdictionary`
--
ALTER TABLE `systemdictionary`
  MODIFY `dictionaryID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识', AUTO_INCREMENT=75;

--
-- 使用表AUTO_INCREMENT `warehouse`
--
ALTER TABLE `warehouse`
  MODIFY `warehouseID` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '自增唯一标识';

--
-- 限制导出的表
--

--
-- 限制表 `contactperson`
--
ALTER TABLE `contactperson`
  ADD CONSTRAINT `fk_contact_customer` FOREIGN KEY (`customerID`) REFERENCES `customer` (`customerID`) ON UPDATE CASCADE;

--
-- 限制表 `customerdeliveryaddress`
--
ALTER TABLE `customerdeliveryaddress`
  ADD CONSTRAINT `fk_address_customer` FOREIGN KEY (`customerID`) REFERENCES `customer` (`customerID`) ON UPDATE CASCADE;

--
-- 限制表 `deliverynote`
--
ALTER TABLE `deliverynote`
  ADD CONSTRAINT `fk_dn_customer` FOREIGN KEY (`customerID`) REFERENCES `customer` (`customerID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_dn_so` FOREIGN KEY (`SalesOrderID`) REFERENCES `salesorder` (`salesOrderID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_dn_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_dn_warehouse` FOREIGN KEY (`WarehouseID`) REFERENCES `warehouse` (`warehouseID`) ON UPDATE CASCADE;

--
-- 限制表 `deliveryproductline`
--
ALTER TABLE `deliveryproductline`
  ADD CONSTRAINT `fk_dline_dn` FOREIGN KEY (`deliveryNoteID`) REFERENCES `deliverynote` (`deliveryNoteID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_dline_product` FOREIGN KEY (`productID`) REFERENCES `product` (`productID`) ON UPDATE CASCADE;

--
-- 限制表 `goodsreceivednote`
--
ALTER TABLE `goodsreceivednote`
  ADD CONSTRAINT `fk_grn_pur` FOREIGN KEY (`PurchaseOrderID`) REFERENCES `purchaseorder` (`purchaseOrderID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_grn_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_grn_supplier` FOREIGN KEY (`supplierID`) REFERENCES `supplier` (`supplierID`) ON UPDATE CASCADE;

--
-- 限制表 `goodsreceivednoterawmaterialline`
--
ALTER TABLE `goodsreceivednoterawmaterialline`
  ADD CONSTRAINT `fk_grnline_grn` FOREIGN KEY (`goodsReceivedNoteID`) REFERENCES `goodsreceivednote` (`goodsReceivedNoteID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_grnline_raw` FOREIGN KEY (`rawMaterialID`) REFERENCES `rawmaterial` (`rawMaterialID`) ON UPDATE CASCADE;

--
-- 限制表 `invoice`
--
ALTER TABLE `invoice`
  ADD CONSTRAINT `fk_inv_customer` FOREIGN KEY (`customerID`) REFERENCES `customer` (`customerID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_inv_so` FOREIGN KEY (`salesOrderID`) REFERENCES `salesorder` (`salesOrderID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_inv_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE;

--
-- 限制表 `invoiceline`
--
ALTER TABLE `invoiceline`
  ADD CONSTRAINT `fk_invline_inv` FOREIGN KEY (`invoiceID`) REFERENCES `invoice` (`invoiceID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_invline_product` FOREIGN KEY (`productID`) REFERENCES `product` (`productID`) ON UPDATE CASCADE;

--
-- 限制表 `paymentvoucher`
--
ALTER TABLE `paymentvoucher`
  ADD CONSTRAINT `fk_pv_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pv_supplier` FOREIGN KEY (`supplierID`) REFERENCES `supplier` (`supplierID`) ON UPDATE CASCADE;

--
-- 限制表 `paymentvoucherpurchaseorder`
--
ALTER TABLE `paymentvoucherpurchaseorder`
  ADD CONSTRAINT `fk_pvpo_po` FOREIGN KEY (`purchaseOrderID`) REFERENCES `purchaseorder` (`purchaseOrderID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pvpo_pv` FOREIGN KEY (`paymentVoucherID`) REFERENCES `paymentvoucher` (`paymentVoucherID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- 限制表 `product`
--
ALTER TABLE `product`
  ADD CONSTRAINT `fk_product_currency` FOREIGN KEY (`currencyID`) REFERENCES `currency` (`currencyID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_product_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE;

--
-- 限制表 `productimage`
--
ALTER TABLE `productimage`
  ADD CONSTRAINT `fk_img_product` FOREIGN KEY (`productID`) REFERENCES `product` (`productID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- 限制表 `productionorder`
--
ALTER TABLE `productionorder`
  ADD CONSTRAINT `fk_po_so` FOREIGN KEY (`salesOrderID`) REFERENCES `salesorder` (`salesOrderID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_po_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE;

--
-- 限制表 `productionorderproductline`
--
ALTER TABLE `productionorderproductline`
  ADD CONSTRAINT `fk_poline_po` FOREIGN KEY (`ProductionOrderID`) REFERENCES `productionorder` (`productionOrderID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_poline_product` FOREIGN KEY (`productID`) REFERENCES `product` (`productID`) ON UPDATE CASCADE;

--
-- 限制表 `productrawmaterialline`
--
ALTER TABLE `productrawmaterialline`
  ADD CONSTRAINT `fk_bom_product` FOREIGN KEY (`productID`) REFERENCES `product` (`productID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_bom_raw` FOREIGN KEY (`rawMaterialID`) REFERENCES `rawmaterial` (`rawMaterialID`) ON UPDATE CASCADE;

--
-- 限制表 `purchaseorder`
--
ALTER TABLE `purchaseorder`
  ADD CONSTRAINT `fk_pur_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_pur_supplier` FOREIGN KEY (`supplierID`) REFERENCES `supplier` (`supplierID`) ON UPDATE CASCADE;

--
-- 限制表 `purchaseorderrawmaterialline`
--
ALTER TABLE `purchaseorderrawmaterialline`
  ADD CONSTRAINT `fk_purline_pur` FOREIGN KEY (`purchaseOrderID`) REFERENCES `purchaseorder` (`purchaseOrderID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_purline_raw` FOREIGN KEY (`rawMaterialID`) REFERENCES `rawmaterial` (`rawMaterialID`) ON UPDATE CASCADE;

--
-- 限制表 `quotation`
--
ALTER TABLE `quotation`
  ADD CONSTRAINT `fk_quote_currency` FOREIGN KEY (`currencyID`) REFERENCES `currency` (`currencyID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_quote_customer` FOREIGN KEY (`customerID`) REFERENCES `customer` (`customerID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_quote_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE;

--
-- 限制表 `quotationproductline`
--
ALTER TABLE `quotationproductline`
  ADD CONSTRAINT `fk_qline_product` FOREIGN KEY (`productID`) REFERENCES `product` (`productID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_qline_quote` FOREIGN KEY (`quotationID`) REFERENCES `quotation` (`quotationID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- 限制表 `rawmaterialrequestnote`
--
ALTER TABLE `rawmaterialrequestnote`
  ADD CONSTRAINT `fk_rmreq_po` FOREIGN KEY (`ProductionOrderID`) REFERENCES `productionorder` (`productionOrderID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_rmreq_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE;

--
-- 限制表 `rawmaterialrequestnoterawmaterial_line`
--
ALTER TABLE `rawmaterialrequestnoterawmaterial_line`
  ADD CONSTRAINT `fk_rmreqline_note` FOREIGN KEY (`rawMaterialRequestNoteID`) REFERENCES `rawmaterialrequestnote` (`rawMaterialRequestNoteID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_rmreqline_raw` FOREIGN KEY (`rawMaterialID`) REFERENCES `rawmaterial` (`rawMaterialID`) ON UPDATE CASCADE;

--
-- 限制表 `rawmaterialshortagereportline`
--
ALTER TABLE `rawmaterialshortagereportline`
  ADD CONSTRAINT `fk_srline_raw` FOREIGN KEY (`rawMaterialID`) REFERENCES `rawmaterial` (`rawMaterialID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_srline_sr` FOREIGN KEY (`shortageReportID`) REFERENCES `shortagereport` (`shortageReportID`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_srline_wh` FOREIGN KEY (`WarehousewarehouseID`) REFERENCES `warehouse` (`warehouseID`) ON UPDATE CASCADE;

--
-- 限制表 `rawmaterialsupplier`
--
ALTER TABLE `rawmaterialsupplier`
  ADD CONSTRAINT `fk_rms_raw` FOREIGN KEY (`rawMaterialID`) REFERENCES `rawmaterial` (`rawMaterialID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_rms_sup` FOREIGN KEY (`supplierID`) REFERENCES `supplier` (`supplierID`) ON UPDATE CASCADE;

--
-- 限制表 `rawmaterialwarehouse`
--
ALTER TABLE `rawmaterialwarehouse`
  ADD CONSTRAINT `fk_rmw_raw` FOREIGN KEY (`rawMaterialID`) REFERENCES `rawmaterial` (`rawMaterialID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_rmw_warehouse` FOREIGN KEY (`warehouseID`) REFERENCES `warehouse` (`warehouseID`) ON UPDATE CASCADE;

--
-- 限制表 `receiptvoucher`
--
ALTER TABLE `receiptvoucher`
  ADD CONSTRAINT `fk_rv_currency` FOREIGN KEY (`currencyID`) REFERENCES `currency` (`currencyID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_rv_customer` FOREIGN KEY (`cusomerID`) REFERENCES `customer` (`customerID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_rv_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE;

--
-- 限制表 `receiptvoucherinvoice`
--
ALTER TABLE `receiptvoucherinvoice`
  ADD CONSTRAINT `fk_rvi_inv` FOREIGN KEY (`invoiceID`) REFERENCES `invoice` (`invoiceID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_rvi_rv` FOREIGN KEY (`receiptVoucherID`) REFERENCES `receiptvoucher` (`receiptVoucherID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- 限制表 `refundrequest`
--
ALTER TABLE `refundrequest`
  ADD CONSTRAINT `fk_refund_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE;

--
-- 限制表 `salesorder`
--
ALTER TABLE `salesorder`
  ADD CONSTRAINT `fk_so_currency` FOREIGN KEY (`currencyCurrencyID`) REFERENCES `currency` (`currencyID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_so_customer` FOREIGN KEY (`customerID`) REFERENCES `customer` (`customerID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_so_staff` FOREIGN KEY (`staffID`) REFERENCES `staff` (`staffID`) ON UPDATE CASCADE;

--
-- 限制表 `salesorderproductline`
--
ALTER TABLE `salesorderproductline`
  ADD CONSTRAINT `fk_soline_product` FOREIGN KEY (`productID`) REFERENCES `product` (`productID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_soline_so` FOREIGN KEY (`salesOrderID`) REFERENCES `salesorder` (`salesOrderID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- 限制表 `systemdictionary_refundrequest`
--
ALTER TABLE `systemdictionary_refundrequest`
  ADD CONSTRAINT `fk_bridge_dict` FOREIGN KEY (`SystemDictionarydictionaryID`) REFERENCES `systemdictionary` (`dictionaryID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_bridge_refund` FOREIGN KEY (`RefundRequestrefundRequestID`) REFERENCES `refundrequest` (`refundRequestID`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- 限制表 `warehouseproduct`
--
ALTER TABLE `warehouseproduct`
  ADD CONSTRAINT `fk_wp_product` FOREIGN KEY (`productID`) REFERENCES `product` (`productID`) ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_wp_warehouse` FOREIGN KEY (`warehouseID`) REFERENCES `warehouse` (`warehouseID`) ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
