#!/usr/bin/python
#! --------------------
#! Script written by Antoine Falempin
#! version 2016-04-20
#! --------------------
import os
import re
import sys, getopt
import fnmatch
import subprocess
import datetime, time #, timedelta
#from datetime import datetime, timedelta
from collections import Counter
cnt = Counter ()

remote = False
sep =';'
filepath = '\\swatch_logs\\'
SuccessfullConnection = "CONNECT_DEVICE | - | Successfully connected to Device"

AppVersionPattern1 = 'App-version: (\S+) [|]'
AppVersionPattern2 = 'App-version: (\S+) [|] Device: (\S+) [|] Model: (\S+) [|] OS: (\S+) [|]'
VersionInfoPattern = 'VersionInformation_t{coolRisc_HW_version=(\d+), coolRisc_SW_version=(\d+), arm_HW_version=(\d+), arm_miniBootLoader_SW_version=(\d+), arm_fotaBootLoader_SW_version=(\d+), arm_app_SW_version=(\d+), stack_HW_version=(\d+), stack_SW_version=(\d+), em6420_HW_version=(\d+), adxl_HW_version=(\d+), em9301_HW_version=(\d+)}'

def GetFileList(folder, date):   
	print ('')
	fileList = []
	filepattern = 'log_'+ date +'*.log'
	print ('path search = ', folder)
	print ('file search', filepattern)
	if (os.path.isdir(folder)):
		if not(os.path.exists(folder)):
			print('Directory does not exist !')
		else:
			#for root, dirs, files in os.walk(path):
			allFiles = os.listdir(folder)
			print ('file found = ', len(allFiles))
			fileList = fnmatch.filter(allFiles, filepattern)
			print ('file matches = ', len(fileList))
		
	print ('')
	return fileList

def ComputeStats(filepath, fileList):

	# only one word permitted -> no space accepted in the chain
	txtpatternDisconnect = 'BluetoothProfile.STATE_DISCONNECTED'
	txtpatternConnected = 'CONNECT_DEVICE'
	txtpatternCallback = 'Callback'
	txtpatternFailure = 'failure'
	txtpatternRepeat = 'Repeat' 
	
	txtpatternFAILED = 'FAILED'
	txtpatternSUCCESS = 'SUCCESS'
	txtpatternUNEXPECTED = 'UNEXPECTED'
	
	tCt = 0	
	tDt = 0
	tCk = 0
	tFl = 0
	tRp = 0
	
	tUNEX = 0
	tFAIL = 0
	tSUCC = 0
	totalLines = 0
	
	lineStatHeader = sep.join(['filename', 'Device', 'AppVersion','coolRisc' , 'ARM' , 'logDate', 'connexion' ,'disconnex' , 'callback','failure','repeat','UNEXPECTED','FAILED','SUCCESS','LineCount'])
	print (lineStatHeader) 
	for file in fileList:
			
		appVersion=''
		deviceVersion = ''
		coolRisc_SW_version =''
		arm_app_SW_version=''
		
		fCnt=0
		fDst=0
		fCbk=0
		fFl =0
		fRp= 0
		
		fUNEX =0
		fFAIL=0
		fSUCC=0
		ftLines=0
		
		ff = filepath + file
		filedate = ''

		# Get file date
		m0 = re.search('(\d{4})(\d{2})(\d{2})[-]', file)
		if m0:
			filedate = '/'.join([m0.group(3),m0.group(2),m0.group(1)])

		#print(ff)
		fileR = open (ff, 'r').read()
		Lines  = fileR.splitlines()
		for line in Lines:
			#lineR = re.split(' |.',line)
			lineR = line.split(' ')
			ftLines+=1
			# version =
			m2 = re.search(AppVersionPattern2, line)
			if m2:
				appVersion = '\''+ m2.group(1)
				deviceVersion = ' '.join([m2.group(2),m2.group(3), 'Android' , m2.group(4)]) # device + model + os
			else:
				m1 = re.search(AppVersionPattern1, line)
				if m1:
					appVersion = '\''+ m1.group(1)
				
			# microP version
			m3 = re.search(VersionInfoPattern, line)
			if m3:
				#print (m2)
				coolRisc_SW_version = '\''+hex(int( m3.group(2)))
				arm_app_SW_version = '\''+hex(int( m3.group(6)))
				#print (coolRisc_SW_version)
				#print (arm_app_SW_version)
				#input("press any key")
				
			fCnt += lineR.count(txtpatternConnected)
			fDst += lineR.count(txtpatternDisconnect)
			fCbk += lineR.count(txtpatternCallback)
			fFl += lineR.count(txtpatternFailure)
			fRp += lineR.count(txtpatternRepeat)
			
			fUNEX += lineR.count(txtpatternUNEXPECTED)
			fFAIL += lineR.count(txtpatternFAILED)
			fSUCC += lineR.count(txtpatternSUCCESS)
		
		tCt+=fCnt
		tDt+=fDst
		tCk+=fCbk
		tFl+=fFl
		tRp+=fRp
		
		tUNEX+=fUNEX
		tFAIL+=fFAIL		
		tSUCC+=fSUCC
		totalLines+=ftLines
				
		lineStat = sep.join([file, deviceVersion, appVersion , coolRisc_SW_version , arm_app_SW_version ,filedate, str(fCnt) ,str(fDst),str(fCbk) ,str(fFl), str(fRp), str(fUNEX), str(fFAIL), str(fSUCC), str(ftLines)])
		print (lineStat)
	print ('------------------------------------- Total ---------------------------------')
	
	#print ( ' Total;;;;;;' +  str(tCt) + ' ;' + str(tDt)+ ' ;' + str(tCk)+ ' ;' + str(tFl)+ ' ;' + str(tUNEX) ,str(tFAIL) ,str(tSUCC))
	totalStat = sep.join(['Total', '', '', '', '', '', str(tCt), str(tDt), str(tCk), str(tFl), str(fRp), str(tUNEX), str(tFAIL) ,str(tSUCC), str(totalLines)])
	print (totalStat)
	print ('')
	
	if (tCt>0):
		print (' Ratio Disconnex = ', '{0:.2%}'.format(tDt/tCt))	
		print (' Ratio Repeat = ', '{0:.2%}'.format(tRp/tCt))
	
	if (tDt>0):
		print (' Ratio Callback = ', '{0:.2%}'.format(tCk/tDt))
		print (' Ratio failure = ', '{0:.2%}'.format(tFl/tDt))
	
	if (totalLines>0):
		print (' Ratio FAILED = ', '{0:.2%}'.format(tFAIL/totalLines))
		print (' Ratio SUCCESS = ', '{0:.2%}'.format(tSUCC/totalLines))
		print (' Ratio UNEXPECTED = ', '{0:.2%}'.format(tUNEX/totalLines))
	else:
		print (' Ratio 0.0%')
	print ('')
	print ('------------------------')
	return

def process2days(date, filepath):
	if len(date)>0:
		print (date)
		oDate = datetime.datetime.strptime(date, '%Y%m%d')
		oYesterDay = datetime.date.fromordinal(oDate.toordinal()-1)
		sYesdate = oYesterDay.strftime("%Y%m%d")
		sDate = oDate.strftime("%Y%m%d")
		
		print ('Previous date = ', sYesdate)
		fileList1 = GetFileList(filepath, sYesdate)
		ComputeStats(filepath, fileList1)
		
		print ('Selected date = ', sDate)
		fileList2 = GetFileList(filepath, sDate)
		ComputeStats(filepath, fileList2)
	
	else:
		print ('Everyday')
		# date = emtpy
		fileList1 = GetFileList(filepath, '')
		ComputeStats(filepath, fileList1)
	return	

def process(remote, date, filepath):	
	#yesterday = datetime.date.today() - timedelta(days=1)
	
	yesterday = datetime.date.fromordinal(datetime.date.today().toordinal()-1)
	yesdate = yesterday.strftime("%Y%m%d")
	today = datetime.date.today()
	todate = today.strftime("%Y%m%d")
	
	print ('Remote process', remote)
	print ('Yesterday=' + yesdate)
	print ('Today=' + todate)
	print ('------------------------')
	print ('Input date is ', date)
	if not date:
		if not (remote): # manual
			date = input('Enter a date, as ' + todate + '\n OR\n just simply press enter :')
			process2days(date, filepath)
		else: # batch
			process2days('', filepath)
	else:
		process2days(date, filepath)
	
	if not (remote):
		input("press any key")

def import_log_from_phone(folderOut):

        if not os.path.isdir(folderOut):
            os.mkdir(folderOut)
            
        __FNULL = open(os.devnull, 'w')

        phoneFolder = '/sdcard/swatch_logs/logs/'
        subprocess.call(['adb', 'pull', phoneFolder, folderOut], stdout=__FNULL, stderr=subprocess.STDOUT)
	
def main(argv):
	global remote, filepath
	date = ''
	rootfolder = '.'
	try:
		opts, args = getopt.getopt(argv,"hdf:",["sdate="])
	except getopt.GetoptError:
		print ('test.py -d <date> -f <folder>')
		sys.exit(2)
	
	if(len(args)>0):	
		remote = True
		
	for opt, arg in opts:
		if opt == '-h':
			print ('test.py -d <date>')
			sys.exit()
		elif opt in ("-d", "--sdate"):
			date = arg
			remote = True
		elif opt in ("-f", "--folder"):
			rootfolder = arg
			remote = True
	
	process(remote, date, rootfolder + filepath)
	#input("press any key")
	return

if __name__ == "__main__":
	main(sys.argv[1:])


# import os
# included_extenstions = ['log', 'lg']
# file_names = [fn for fn in os.listdir(relevant_path)
		# if any(fn.endswith(ext) for ext in included_extenstions)]
	
# for file in fileList:
	# words = re.findall(r'\w+', open(filepath + file).read().lower())
# word_counts = Counter(words)
# top_three = word_counts.most_common(3)
# print(top_three)
# input("press any key")
	
	#cnt[file]+=1
	#cnt[file]+= fCnt
	# for line in open (filepath + file, 'r'):
		# for word in line.split():
			# cnt[word]+=1
		
# for i, v  in enumerate(cnt, start=1):   # Python indexes start at zero
    # print (v, i)

#print cnt
#print('\n'.join(cnt))
#print(*cnt)
#print str(cnt)[1:-1]